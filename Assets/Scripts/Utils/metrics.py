import csv
import time
from collections import defaultdict
from enum import Enum
from math import ceil, sqrt

import yaml
import matplotlib.pyplot as plt
import numpy as np


class Layouts(Enum):
    SliderOnly = "SliderOnly"
    ArcType = "ArcType"
    TiltType = "TiltType"
    Raycast = "Raycast"


trial_files = {0: "trial-2020-07-16_02-36-27.yaml", 1: "trial-2020-07-16_02-46-22.yaml",
               2: "trial-2020-07-16_03-13-40.yaml", 3: "trial-2020-07-16_03-36-35.yaml"}


def get_all_trials():
    return {v: get_trial(k) for k, v in trial_files.items()}


def get_trial(n=0):
    with open("../../Results/" + trial_files[n]) as yams:
        return yaml.load(yams, Loader=yaml.UnsafeLoader)


def print_yaml_recur(yams, indent=0):
    if isinstance(yams, list):
        for v in yams:
            print_yaml_recur(v, indent + 1)
    elif not isinstance(yams, dict):
        print("  " * indent, yams)
    else:
        for k, v in yams.items():
            print("  " * indent, k, ': ', sep='', end='')
            inline = isinstance(v, list) and all(isinstance(i, float) or isinstance(i, int) for i in v)
            inline = inline or isinstance(v, str) or isinstance(v, float) or isinstance(v, int)
            if inline:
                print(v)
            else:
                print()
                print_yaml_recur(v, indent + 1)


def randrange(n, vmin, vmax):
    return (vmax - vmin) * np.random.rand(n) + vmin


def extract_layout_positions(data: dict, layout):
    out = list()
    for item in data["trial"]:
        try:
            if item["challenge"]["layout"] == layout:
                for kp in item["challenge"]["keypresses"].values():
                    out.append(kp["pressPos"])
        except KeyError:
            pass
        except AttributeError:
            pass
    return out


def merge_trials(*datas):
    out = list()
    for t in datas:
        out.extend(t["trial"])
    return {"meta": "lost in merge", "trial": out}


arctype_x_range_size = 145 - 45
tilttype_x_range_size = 12 - 5
tilttype_z_range_size = 120 - 30


def get_tilttype_pos(c: str):
    if c == ' ':
        return 6, 3
    if not c.isalpha():
        raise ValueError()
    delta = (ord(c.lower()) - ord('a'))
    return delta // 4, delta % 4


def get_arctype_bin(c: str):
    if c == ' ':
        return 7
    if not c.isalpha():
        raise ValueError()
    return (ord(c.lower()) - ord('a')) // 4


def arctype_ideal(prompt: str):
    def get_ideal_travel(p, c):
        bin_delta = abs(get_arctype_bin(p) - get_arctype_bin(c))
        ideal_travel_per_bin = arctype_x_range_size / ceil(26 / 4)
        return bin_delta * ideal_travel_per_bin

    ideal = 0
    last = None
    for c in prompt:
        if last is not None:
            ideal += get_ideal_travel(last, c)
        last = c

    return ideal


def tilttype_ideal_tuple(prompt: str):
    def get_ideal_travel(p, c):
        x, z = (abs(a - b) for a, b in zip(get_tilttype_pos(p), get_tilttype_pos(c)))
        return x * tilttype_x_range_size / ceil(26 / 4), z * tilttype_z_range_size / 4

    ideal = (0, 0)
    last = None
    for c in prompt:
        if last is not None:
            ideal = (a + b for a, b in zip(ideal, get_ideal_travel(last, c)))
        last = c

    return tuple(ideal)


def tilttype_ideal(prompt: str):
    a, b = tilttype_ideal_tuple(prompt)
    return sqrt(a * a + b * b)


def challenge_rot_travel(challenge):
    out = 0
    for kp in challenge['keypresses'].values():
        try:
            out += sqrt(sum(k * k for k in kp['travel']['rot']))
        except KeyError:
            pass

    return out


def words_per_minute(challenge):
    interval = challenge["time"]["duration"]
    assert interval > 0
    minutes_of_entry = interval / 60
    output = challenge["output"]
    words_entered = max(0, len(output) - 1) / 5
    return words_entered / minutes_of_entry


def accurate_words_per_minutes(challenge):
    prompt = challenge["prompt"]
    a = sum(1 for a, b in zip(challenge["output"], prompt) if a == b) / len(prompt)
    return a * words_per_minute(challenge)


# https://stackoverflow.com/questions/11686720/is-there-a-numpy-builtin-to-reject-outliers-from-a-list
def reject_outliers(data, m=2):
    return data[abs(data - np.mean(data)) < m * np.std(data)]


def make_point_cloud():
    merged = merge_trials(*get_all_trials().values())
    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')

    for layout, marker in zip((e.value for e in Layouts), ['o', '^', '.', 's']):
        posses = extract_layout_positions(merged, layout)
        if not posses:
            continue

        xs = [v[0] for v in posses]
        ys = [v[1] for v in posses]
        zs = [v[2] for v in posses]
        ax.scatter(xs, ys, zs, marker=marker)

        ax.set_autoscalex_on(False)
        ax.set_xlim([-4, 4])

        ax.set_xlabel('X (feet)')
        ax.set_ylabel('Y (feet)')
        ax.set_zlabel('Z (feet)')

    plt.legend([e.value for e in Layouts], loc='center left')
    plt.title('Stylus Positions in Cave on Keypress by Interface')
    plt.savefig("../../Results/Figures/trnsprnt-pos-cloud.png", transparent=True)
    plt.savefig("../../Results/Figures/pos-cloud.png")
    plt.show()


def make_2d_point_cloud():
    merged = merge_trials(*get_all_trials().values())
    fig = plt.figure()
    ax = fig.add_subplot(111)

    for layout, marker in zip((e.value for e in Layouts), ['o', '^', '.', 's']):
        posses = extract_layout_positions(merged, layout)
        if not posses:
            continue

        xs = [v[0] for v in posses]
        ys = [v[2] for v in posses]

        ax.scatter(xs, ys, marker=marker)
        ax.plot([np.mean(xs)], [np.mean(ys)], marker='x')

        ax.set_autoscalex_on(False)
        ax.set_xlim([-1, 1])

        ax.set_xlabel('X (feet)')
        ax.set_ylabel('Z (feet)')

    plt.legend([e.value for e in Layouts], loc='center left')
    plt.title('Stylus Positions in Cave on Keypress by Interface')
    plt.savefig("../../Results/Figures/trnsprnt-pos-2d-cloud.png", transparent=True)
    plt.savefig("../../Results/Figures/pos-2d-cloud.png")
    plt.show()


def make_wpm_bars():
    layout_wpm, layout_awpm = defaultdict(list), defaultdict(list)
    for trial in get_all_trials().values():
        for challenge in trial["trial"]:
            if "command" in challenge:
                continue
            else:
                challenge = challenge["challenge"]
            layout_wpm[challenge["layout"]].append(words_per_minute(challenge))
            layout_awpm[challenge["layout"]].append(accurate_words_per_minutes(challenge))

    awpm_data = [reject_outliers(np.array(v)) for v in layout_awpm.values()]
    awpm_means = [np.mean(v) for v in awpm_data]
    wpm_data = [reject_outliers(np.array(v)) for v in layout_wpm.values()]
    wpm_means = [np.mean(v) for v in wpm_data]
    awpm_std = [np.std(v) for v in awpm_data]
    print('awpm std', awpm_std[-1])
    awpm_std[-1] = 0  # overlaps with other error bar
    wpm_std = [np.std(v) for v in wpm_data]
    ind = np.arange(4)  # the x locations for the groups
    width = 0.35  # the width of the bars: can also be len(x) sequence

    p1 = plt.bar(ind, awpm_means, width, yerr=awpm_std, align='center')
    p2 = plt.bar(ind, wpm_means, width, align='center',
                 bottom=awpm_means, yerr=wpm_std)

    plt.axes().yaxis.grid(True)  # raises an exception?

    plt.ylabel('(Accurate) Words per Minute')
    plt.title('Efficiency')
    plt.xticks(ind, layout_awpm.keys())
    plt.legend((p1[0], p2[0]), ('aWPM', 'WPM'))

    plt.savefig('../../Results/Figures/stacked-wpm-awpm.png')
    plt.show()


def make_pit_bars():
    layout_pit = defaultdict(list)
    for trial in get_all_trials().values():
        for challenge in trial["trial"]:
            if "command" in challenge:
                continue
            else:
                challenge = challenge["challenge"]

            layout = challenge['layout']
            if layout == Layouts.ArcType.value:
                layout_pit[layout].append((challenge_rot_travel(challenge), arctype_ideal(challenge['prompt'])))
            elif layout == Layouts.TiltType.value:
                layout_pit[layout].append((challenge_rot_travel(challenge), tilttype_ideal(challenge['prompt']),
                                           tilttype_ideal_tuple(challenge['prompt'])))

    # https://matplotlib.org/3.1.1/gallery/lines_bars_and_markers/bar_stacked.html
    idealMeans = [np.mean(reject_outliers(np.array([t[1] for t in layout_pit[v]]))) for v in
                  [Layouts.ArcType.value, Layouts.TiltType.value]]
    actualMeans = [np.mean(reject_outliers(np.array([t[0] for t in layout_pit[v]]))) for v in
                   [Layouts.ArcType.value, Layouts.TiltType.value]]
    idealStd = [np.std(reject_outliers(np.array([t[1] for t in layout_pit[v]]))) for v in
                [Layouts.ArcType.value, Layouts.TiltType.value]]
    actualStd = [np.std(reject_outliers(np.array([t[0] for t in layout_pit[v]]))) for v in
                 [Layouts.ArcType.value, Layouts.TiltType.value]]
    ind = np.arange(2)  # the x locations for the groups
    width = 0.85  # the width of the bars: can also be len(x) sequence

    p1 = plt.bar(ind, idealMeans, width, yerr=idealStd, align='center')
    p2 = plt.bar(ind, actualMeans, width, align='center',
                 bottom=idealMeans, yerr=actualStd)

    plt.ylabel('Travel')
    plt.title('Travel by Interface')
    plt.xticks(ind, (Layouts.ArcType.value, Layouts.TiltType.value))
    plt.legend((p1[0], p2[0]), ('Ideal', 'Actual'))

    plt.savefig('../../Results/Figures/travel-by-interface-error-bars.png')
    plt.show()

    pitMeans = [np.mean(reject_outliers(np.array([100 * t[0] / t[1] for t in layout_pit[v]]))) for v in
                [Layouts.ArcType.value, Layouts.TiltType.value]]
    pitStd = [np.std(reject_outliers(np.array([100 * t[0] / t[1] for t in layout_pit[v]]))) for v in
              [Layouts.ArcType.value, Layouts.TiltType.value]]
    # ind = np.arange(2)  # the x locations for the groups
    # width = 0.85  # the width of the bars: can also be len(x) sequence

    fig, ax = plt.subplots()
    ax.bar(ind, pitMeans, yerr=pitStd, align='center', alpha=0.5, ecolor='black', capsize=10)
    ax.set_ylabel('PIT')
    plt.xticks(ind, (Layouts.ArcType.value, Layouts.TiltType.value))
    ax.set_title('PIT by Interface')
    ax.yaxis.grid(True)

    plt.savefig('../../Results/Figures/pit-by-interface-error-bars.png')
    plt.show()


def write_csv():
    layout_pit, layout_wpm, layout_awpm = defaultdict(list), defaultdict(list), defaultdict(list)
    for trial in get_all_trials().values():
        for challenge in trial["trial"]:
            if "command" in challenge:
                continue
            else:
                challenge = challenge["challenge"]

            layout_wpm[challenge["layout"]].append(words_per_minute(challenge))
            layout_awpm[challenge["layout"]].append(accurate_words_per_minutes(challenge))

            layout = challenge['layout']
            if layout == Layouts.ArcType.value:
                layout_pit[layout].append((challenge_rot_travel(challenge), arctype_ideal(challenge['prompt'])))
            elif layout == Layouts.TiltType.value:
                layout_pit[layout].append((challenge_rot_travel(challenge), tilttype_ideal(challenge['prompt']),
                                           tilttype_ideal_tuple(challenge['prompt'])))

    header = ["Layout", "Avg X", "Std Dev X", "Avg Y", "Std Dev Y", "Avg Z", "Std Dev Z", "Avg WPM", "Std Dev WPM", "Avg aWPM", "Std Dev aWPM", "Avg Travel",
            "Std Dev Travel", "Avg PIT", "Std Dev PIT"]
    rows = [header]
    merged = merge_trials(*get_all_trials().values())
    for layout in (e.value for e in Layouts):
        posses = extract_layout_positions(merged, layout)
        row = [layout]
        if posses:
            xs = [v[0] for v in posses]
            ys = [v[1] for v in posses]
            zs = [v[2] for v in posses]
            row.append(np.mean(xs))
            row.append(np.std(xs))
            row.append(np.mean(ys))
            row.append(np.std(ys))
            row.append(np.mean(zs))
            row.append(np.std(zs))
        else:
            row.extend(('', '', '', '', '', ''))

        wpm_data = reject_outliers(np.array(layout_wpm[layout]))
        row.append(np.mean(wpm_data))
        row.append(np.std(wpm_data))

        awpm_data = reject_outliers(np.array(layout_awpm[layout]))
        row.append(np.mean(awpm_data))
        row.append(np.std(awpm_data))

        if layout in layout_pit:
            actual_data = reject_outliers(np.array([t[0] for t in layout_pit[layout]]))
            row.append(np.mean(actual_data))
            row.append(np.std(actual_data))

            pit_data = reject_outliers(np.array([100 * t[0] / t[1] for t in layout_pit[layout]]))

            row.append(np.mean(pit_data))
            row.append(np.std(pit_data))
        else:
            row.extend(('', '', '', ''))

        rows.append(row)

    with open('../../Results/extracted_data.csv', 'w', newline='') as csvfile:
        csv.writer(csvfile).writerows(rows)


# arctype, arc x: 60 / 7 per bin
# raycast, key width 0.701 height 0.461
# twoaxis, x: (180 + 175) / 4 per letter vert, z: (168 + 175) / 10 per letter horiz
if __name__ == '__main__':
    # make_point_cloud()
    # make_wpm_bars()
    # make_2d_point_cloud()
    make_pit_bars()
    write_csv()
