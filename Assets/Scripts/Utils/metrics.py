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


trial_files = {0: "trial-2020-07-16_02-36-27.yaml", 1: "trial-2020-07-16_02-46-22.yaml", 2: "trial-2020-07-16_03-13-40.yaml", 3: "trial-2020-07-16_03-36-35.yaml"}


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


arctype_x_range_size = 60
tilttype_x_range_size = 175 + 160
tilttype_z_range_size = 168 + 175


def get_tilttype_pos(c: str):
    if c == ' ':
        return 7
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


def tilttype_ideal(prompt: str):
    def get_ideal_travel(p, c):
        x, z = (abs(a - b) for a, b in zip(get_tilttype_pos(p), get_tilttype_pos(c)))
        return x * tilttype_x_range_size / ceil(26 / 4), z * tilttype_z_range_size / 4

    ideal = (0, 0)
    last = None
    for c in prompt:
        if last is not None:
            ideal = (a + b for a, b in zip(ideal, get_ideal_travel(last, c)))
        last = c

    return ideal


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
        # ax.set_autoscaley_on(False)
        # ax.set_autoscalez_on(False)
        # ax.set_zlim([-1.5, 1])

        ax.set_xlabel('X (feet)')
        ax.set_ylabel('Y (feet)')
        ax.set_zlabel('Z (feet)')

    plt.legend([e.value for e in Layouts])
    t_str = str(time.time())
    plt.savefig("../../Results/Figures/trnsprnt-pos-cloud.png", transparent=True)
    plt.savefig("../../Results/Figures/pos-cloud.png")
    plt.show()

def make_wpm_bars():
    layout_wpm, layout_awpm = defaultdict(list), defaultdict(list)
    for trial in get_all_trials().values():
        for challenge in trial["trial"]:
            if "command" in challenge:
                continue
            challenge = challenge["challenge"]
            layout_wpm[challenge["layout"]].append(words_per_minute(challenge))
            layout_awpm[challenge["layout"]].append(accurate_words_per_minutes(challenge))

    for title, data in [("WPM", layout_wpm), ("aWPM", layout_awpm)]:
        data = {k: reject_outliers(np.array(v)) for k, v in data.items()}
        layouts = data.keys()
        means = list(map(np.mean, data.values()))
        stds = list(map(np.std, data.values()))
        x_pos = np.arange(len(layouts))

        # https://pythonforundergradengineers.com/python-matplotlib-error-bars.html
        fig, ax = plt.subplots()
        ax.bar(x_pos, means, yerr=stds, align='center', alpha=0.5, ecolor='black', capsize=10)
        ax.set_ylabel(title)
        ax.set_xticks(x_pos)
        ax.set_xticklabels(layouts)
        ax.set_title(title + ' by Layout')
        ax.yaxis.grid(True)

        plt.tight_layout()
        plt.savefig('../../Results/Figures/' + title.lower() + '-error-bars.png')
        plt.show()


# arctype, arc x: 60 / 7 per bin
# raycast, key width 0.701 height 0.461
# twoaxis, x: (180 + 175) / 4 per letter vert, z: (168 + 175) / 10 per letter horiz
if __name__ == '__main__':
    # make_point_cloud()
    # make_wpm_bars()

    pass