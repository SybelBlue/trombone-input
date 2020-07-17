import csv
import time
from collections import defaultdict
from enum import Enum
from math import ceil, sqrt, trunc

import yaml
import matplotlib.pyplot as plt
import numpy as np


def use(fn, *args, **kwargs):
    def access(obj):
        return getattr(obj, fn)(*args, **kwargs)

    return access


def truncate(f, digits=5):
    return trunc(f * 10 ** digits) / 10 ** digits


# https://stackabuse.com/levenshtein-distance-and-text-similarity-in-python/
def levenshtein(seq1, seq2):
    size_x = len(seq1) + 1
    size_y = len(seq2) + 1
    matrix = np.zeros((size_x, size_y))
    for x in range(size_x):
        matrix[x, 0] = x
    for y in range(size_y):
        matrix[0, y] = y

    for x in range(1, size_x):
        for y in range(1, size_y):
            if seq1[x - 1] == seq2[y - 1]:
                matrix[x, y] = min(
                    matrix[x - 1, y] + 1,
                    matrix[x - 1, y - 1],
                    matrix[x, y - 1] + 1
                )
            else:
                matrix[x, y] = min(
                    matrix[x - 1, y] + 1,
                    matrix[x - 1, y - 1] + 1,
                    matrix[x, y - 1] + 1
                )

    return matrix[size_x - 1, size_y - 1]


def error_rate(P, T):
    return 100 * levenshtein(P, T) / max(len(P), len(T))


class Layouts(Enum):
    SliderOnly = "SliderOnly"
    ArcType = "ArcType"
    TiltType = "TiltType"
    Raycast = "Raycast"


def layout_name(l):
    if isinstance(l, str):
        return {Layouts.SliderOnly.value: 'DO NOT USE', Layouts.ArcType.value: Layouts.ArcType.value,
                Layouts.TiltType.value: Layouts.TiltType.value, Layouts.Raycast.value: 'Controller Pointing'}[l]
    return {Layouts.SliderOnly: 'DO NOT USE', Layouts.ArcType: Layouts.ArcType.value,
                Layouts.TiltType: Layouts.TiltType.value, Layouts.Raycast: 'Controller Pointing'}[l]


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


def extract_layout_positions(data: dict, layout, use_practice=False):
    out = list()
    for item in data["trial"]:
        try:
            challenge = item["challenge"]
            if challenge["layout"] == layout and (not use_practice or not challenge['type'] == 'Practice'):
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
tilttype_x_range_size = 30 - 5
tilttype_z_range_size = 30 - -10


def tilttype_pos(c: str):
    if c == ' ':
        return 6, 3
    if not c.isalpha():
        raise ValueError()
    delta = (ord(c.lower()) - ord('a'))
    return delta // 4, delta % 4


def raycast_pos(c: str):
    for i, row in enumerate(['QWERTYUIOP', 'ASDFGHJKL;', 'ZXCVBNM,. ']):
        if c.upper() in row:
            return row.find(c), i


def raycast_displacement(p, c, signed=False):
    return ((a - b if signed else abs(a - b)) for a, b in zip(raycast_pos(p), raycast_pos(c)))


def arctype_bin(c: str):
    if c == ' ':
        return 7
    if not c.isalpha():
        raise ValueError()
    return (ord(c.lower()) - ord('a')) // 4


def arctype_ideal(prompt: str):
    def get_ideal_travel(p, c):
        bin_delta = abs(arctype_bin(p) - arctype_bin(c))
        ideal_travel_per_bin = arctype_x_range_size / ceil(26 / 4)
        return bin_delta * ideal_travel_per_bin

    ideal = 0
    last = None
    for c in prompt:
        if last is not None:
            ideal += get_ideal_travel(last, c)
        last = c

    return ideal


def tilttype_displacement(p, c, signed=False):
    return ((a - b if signed else abs(a - b)) for a, b in zip(tilttype_pos(p), tilttype_pos(c)))


def tilttype_ideal_tuple(prompt: str):
    def get_ideal_travel(p, c):
        x, z = tilttype_displacement(p, c)
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
    # start = list(challenge["keypresses"].keys())[0] if challenge["keypresses"] else challenge["time"]["start"]
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


def make_point_cloud(data):
    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')

    for layout, marker in zip((e.value for e in Layouts), ['o', '^', '.', 's']):
        posses = data.layout_posses[layout]
        if not posses or layout == Layouts.SliderOnly.value:
            continue

        xs = [v[0] for v in posses]
        ys = [v[1] for v in posses]
        zs = [v[2] for v in posses]
        ax.scatter(xs, zs, ys, marker=marker)

        ax.set_autoscalex_on(False)
        ax.set_xlim([-4.5, 1])
        ax.set_ylim([0, 5.5])
        ax.set_zlim([-1, 4.5])

        ax.set_xlabel('X (ft)')
        ax.set_ylabel('Z (ft)')
        ax.set_zlabel('Y (ft)')

    plt.legend([layout_name(e) for e in Layouts if e != Layouts.SliderOnly], loc='center left')
    # plt.title('Stylus Positions in Cave on Keypress by Interface')
    plt.savefig("../../Results/Figures/pos-cloud.png", transparent=True)
    plt.show()


def make_2d_point_cloud(data, axes):
    fig = plt.figure()
    ax = fig.add_subplot(111)

    for layout, marker in zip((e.value for e in Layouts), ['o', '^', '.', 's']):
        posses = data.layout_posses[layout]
        if not posses or layout == Layouts.SliderOnly.value:
            continue

        xs = [v[axes[0]] for v in posses]
        ys = [v[axes[1]] for v in posses]

        ax.scatter(xs, ys, marker=marker)
        # ax.plot(np.mean(xs), np.mean(ys), marker='H')

        # ax.set_autoscalex_on(False)
        # ax.set_xlim([-1, 1])

        axes_labels = ['X', 'Z', 'Y']
        ax.set_xlabel(axes_labels[axes[0]] + ' (feet)')
        ax.set_ylabel(axes_labels[axes[1]] + ' (feet)')

    plt.legend([layout_name(e) for e in Layouts if e != Layouts.SliderOnly], loc='center left')
    # plt.title('Stylus Positions in Cave on Keypress by Interface')
    plt.savefig("../../Results/Figures/pos-2d-cloud-" + '-'.join(map(str, axes)) + ".png", transparent=True)
    # plt.savefig("../../Results/Figures/pos-2d-cloud-" + '-'.join(map(str, axes)) + ".png")
    plt.show()


def make_wpm_bars(data):
    perfect_wpm = {k: v for k, v in data.layout_perfect_wpm.items() if k != Layouts.SliderOnly.value}
    blind_wpm = {k: v for k, v in data.layout_blind_wpm.items() if k != Layouts.SliderOnly.value}
    perfect_data = perfect_wpm.values()
    perfect_means = [np.mean(v) for v in perfect_data]
    blind_data = blind_wpm.values()
    blind_means = [np.mean(v) for v in blind_data]
    perfect_std = [np.std(v) for v in perfect_data]
    blind_std = [np.std(v) for v in blind_data]
    ind = np.arange(len(blind_data))  # the x locations for the groups
    width = 0.35  # the width of the bars: can also be len(x) sequence

    p1 = plt.bar(ind + width, perfect_means, width, align='center', yerr=perfect_std)
    p2 = plt.bar(ind, blind_means, width, align='center', yerr=blind_std)

    plt.axes().yaxis.grid(True)  # raises an error?

    plt.ylabel('WPM')
    # plt.title('Efficiency')
    plt.xticks(ind + width/2, list(map(layout_name, blind_wpm.keys())))
    plt.legend((p1[0], p2[0]), ('Perfect', 'Blind'))

    plt.savefig('../../Results/Figures/perfect-blind-wpm.png', transparent=True)
    plt.show()


def make_pit_bars(data):
    # https://matplotlib.org/3.1.1/gallery/lines_bars_and_markers/bar_stacked.html
    layouts = [Layouts.ArcType.value, Layouts.TiltType.value]
    ideal_means = [np.mean(data.layout_ideal_travel[v]) for v in layouts]
    actual_means = [np.mean(data.layout_actual_travel[v]) for v in layouts]
    ideal_std = [np.std(data.layout_ideal_travel[v]) for v in layouts]
    actual_std = [np.std(data.layout_actual_travel[v]) for v in layouts]
    ind = np.arange(2)  # the x locations for the groups
    width = 0.85  # the width of the bars: can also be len(x) sequence

    p1 = plt.bar(ind, ideal_means, width, yerr=ideal_std, align='center')
    p2 = plt.bar(ind, [a - b for a, b in zip(actual_means, ideal_means)], width, align='center',
                 bottom=ideal_means, yerr=actual_std)

    plt.ylabel('Total Angular Displacement (degrees)')
    # plt.title('Travel by Interface')
    plt.xticks(ind, (Layouts.ArcType.value, Layouts.TiltType.value))
    plt.legend((p1[0], p2[0]), ('Ideal', 'Actual'))

    plt.savefig('../../Results/Figures/travel-by-interface-error-bars.png', transparent=True)
    plt.show()

    pit_means = [np.mean(data.layout_pit[v]) for v in layouts]
    pit_std = [np.std(data.layout_pit[v]) for v in layouts]
    # ind = np.arange(2)  # the x locations for the groups
    # width = 0.85  # the width of the bars: can also be len(x) sequence

    fig, ax = plt.subplots()
    ax.bar(ind, pit_means, yerr=pit_std, align='center', alpha=0.5, ecolor='black', capsize=10)
    ax.set_ylabel('PIT (%)')
    plt.xticks(ind, layouts)
    # ax.set_title('PIT by Interface')
    ax.yaxis.grid(True)

    plt.savefig('../../Results/Figures/pit-by-interface-error-bars.png', transparent=True)
    plt.show()


def make_duration_lines(data):
    fig, ax = plt.subplots()
    items = list()
    for layout, durations in data.layout_durations.items():
        if layout != Layouts.SliderOnly.value:
            items.append(plt.plot(np.arange(len(durations)), durations))

    plt.legend(items, (e.value for e in Layouts if e != Layouts.SliderOnly))
    fig.show()


def make_error_bars(data):
    items = list()
    for layout, pairs in data.layout_blind_io.items():
        if layout == Layouts.SliderOnly.value:
            continue
        errors = 0
        off_by_one = 0
        dipped = 0
        for prompt, output in pairs:
            for p, o in zip(prompt, output):
                if layout == Layouts.TiltType.value or layout == Layouts.ArcType.value:
                    a, b = tilttype_displacement(p, o, signed=True)
                else:
                    a, b = raycast_displacement(p, o, signed=True)
                if b == -1:
                    dipped += 1
                if abs(a) + abs(b) == 1:
                    off_by_one += 1
                if p != o:
                    errors += 1
        items.append((layout, 100 * dipped / errors, 100 * off_by_one / errors))
        # print((layout, dipped, off_by_one, errors))
    off_by_one_pcts = [obo for _, _, obo in items]
    dipped_pcts = [dip for _, dip, _ in items]
    ind = np.arange(3)  # the x locations for the groups
    width = 0.5  # the width of the bars: can also be len(x) sequence

    p1 = plt.bar(ind, dipped_pcts, width, align='center')[0]
    p2 = plt.bar(ind, [a - b for a, b in zip(off_by_one_pcts, dipped_pcts)], width, align='center', bottom=dipped_pcts)[
        0]

    plt.ylabel('Percent of Errors')
    # plt.title('Travel by Interface')
    plt.xticks(ind, (layout_name(e) for e in data.layout_blind_io.keys() if e != Layouts.SliderOnly.value))
    plt.xlabel("Layouts")
    plt.legend((p1, p2), ('Dipped (y + 1)', 'Off-by-One'))

    plt.savefig('../../Results/Figures/error-chart.png', transparent=True)
    plt.show()


def get_data(skip_practice=True):
    layout_pit, layout_blind_wpm, layout_blind_awpm, layout_perfect_wpm, layout_blind_io, layout_durations = \
        (defaultdict(list) for _ in range(6))
    layout_actual_travel, layout_ideal_travel, layout_error_rates = (dict() for _ in range(3))
    trials = get_all_trials()
    for trial in trials.values():
        for challenge in trial["trial"]:
            if "command" in challenge:
                continue
            else:
                challenge = challenge["challenge"]

            if challenge['type'] == 'Practice' and skip_practice:
                continue

            if challenge['type'] == 'Blind':
                layout_blind_wpm[challenge["layout"]].append(words_per_minute(challenge))
                layout_blind_awpm[challenge["layout"]].append(accurate_words_per_minutes(challenge))

            if challenge['type'] == 'Perfect':
                layout_perfect_wpm[challenge["layout"]].append(words_per_minute(challenge))

            layout = challenge['layout']
            if layout == Layouts.ArcType.value:
                layout_pit[layout].append((challenge_rot_travel(challenge), arctype_ideal(challenge['prompt'])))
            elif layout == Layouts.TiltType.value:
                layout_pit[layout].append((challenge_rot_travel(challenge), tilttype_ideal(challenge['prompt']),
                                           tilttype_ideal_tuple(challenge['prompt'])))

            layout_durations[layout].append(challenge['time']['duration'])

            if challenge['type'] == 'Blind':
                layout_blind_io[layout].append((challenge['prompt'], challenge['output']))

    merged = merge_trials(*trials.values())
    layout_posses = {e.value: extract_layout_positions(merged, e.value) for e in Layouts}
    travel_csv = [["Layout", "Avg Travel", "Std Dev Travel", "Avg PIT", "Std Dev PIT"]]
    main_csv = [["Layout", "Avg BlindWPM", "Std Dev BlindWPM", "Avg PerfectWPM", "Std Dev PerfectWPM",
                 "Avg Error", "Std Dev Error"]]
    for layout in (e.value for e in Layouts):
        row = [layout]

        for l_dict in [layout_blind_wpm, layout_perfect_wpm]:
            wpm_data = reject_outliers(np.array(l_dict[layout]))
            l_dict[layout] = wpm_data
            row.append(np.mean(wpm_data))
            row.append(np.std(wpm_data))

        awpm_data = reject_outliers(np.array(layout_blind_awpm[layout]))
        layout_blind_awpm[layout] = awpm_data
        # row.append(np.mean(awpm_data))
        # row.append(np.std(awpm_data))

        dur_data = reject_outliers(np.array(layout_durations[layout]))
        layout_durations[layout] = dur_data

        error_data = [error_rate(*p) for p in layout_blind_io[layout]]
        layout_error_rates[layout] = error_data
        row.append(np.mean(error_data))
        row.append(np.std(error_data))

        if layout in layout_pit:
            actual_data = reject_outliers(np.array([t[0] for t in layout_pit[layout]]))
            layout_actual_travel[layout] = actual_data

            ideal_data = reject_outliers(np.array([t[1] for t in layout_pit[layout]]))
            layout_ideal_travel[layout] = ideal_data

            pit_data = reject_outliers(np.array([100 * t[0] / t[1] for t in layout_pit[layout]]))
            layout_pit[layout] = pit_data

            travel_csv.append([layout, np.mean(actual_data), np.std(actual_data), np.mean(pit_data), np.std(pit_data)])

        main_csv.append(row)

    return Data(main_csv=main_csv, layout_pit=layout_pit, layout_blind_wpm=layout_blind_wpm,
                layout_blind_awpm=layout_blind_awpm, layout_posses=layout_posses,
                layout_ideal_travel=layout_ideal_travel, layout_actual_travel=layout_actual_travel,
                layout_durations=layout_durations, layout_blind_io=layout_blind_io,
                layout_error_rates=layout_error_rates, travel_csv=travel_csv,
                layout_perfect_wpm=layout_perfect_wpm)


class Data:
    def __init__(self, **kwargs):
        self.main_csv = kwargs['main_csv']
        self.travel_csv = kwargs['travel_csv']
        self.layout_pit = kwargs['layout_pit']
        self.layout_blind_wpm = kwargs['layout_blind_wpm']
        self.layout_perfect_wpm = kwargs['layout_perfect_wpm']
        self.layout_blind_awpm = kwargs['layout_blind_awpm']
        self.layout_posses = kwargs['layout_posses']
        self.layout_actual_travel = kwargs['layout_actual_travel']
        self.layout_ideal_travel = kwargs['layout_ideal_travel']
        self.layout_durations = kwargs['layout_durations']
        self.layout_blind_io = kwargs['layout_blind_io']
        self.layout_error_rates = kwargs['layout_error_rates']


def write_csv(name, rows, digits=2):
    with open('../../Results/' + name + '.csv', 'w', newline='') as csvfile:
        rows = [[(truncate(x, digits=digits) if isinstance(x, float) else x) for x in row] for row in rows]
        csv.writer(csvfile).writerows(rows)


if __name__ == '__main__':
    data = get_data()
    make_point_cloud(data)
    make_2d_point_cloud(data, [0, 2])
    make_2d_point_cloud(data, [1, 2])

    make_wpm_bars(data)
    make_pit_bars(data)
    # make_duration_lines(data)  # broken

    make_error_bars(data)

    write_csv('main', data.main_csv, digits=2)
    write_csv('travels', data.travel_csv, digits=2)

    merged = merge_trials(*[get_trial(n) for n in range(3)])['trial']
    prompts = list()
    for challenge in merged:
        if 'command' in challenge:
            continue
        prompts.append(challenge['challenge']['prompt'])

    chars_per_prompt = list(map(len, prompts))

    print('chars per prompt', np.mean(chars_per_prompt), np.std(chars_per_prompt))

    words_per_prompt = list(map(len, map(use('split'), prompts)))

    print('words per prompt', np.mean(words_per_prompt), np.std(words_per_prompt))

    chars = [defaultdict(int) for _ in range(6)]
    for prompt in prompts:
        for word in prompt.split():
            for i in range(min(len(word), len(chars))):
                chars[i][word[i]] += 1


# split wpm for blind and perfect, no awpm
# update ranges, regenerate all
# rename raycast
