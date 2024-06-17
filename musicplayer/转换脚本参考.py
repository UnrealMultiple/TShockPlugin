import pretty_midi

# 定义NoteDictionary
NoteDictionary = {
    "C4": -1.0,
    "C#4": -0.916667,
    "D4": -0.833333,
    "D#4": -0.75,
    "E4": -0.666667,
    "F4": -0.583333,
    "F#4": -0.5,
    "G4": -0.416667,
    "G#4": -0.333333,
    "A4": -0.25,
    "A#4": -0.166667,
    "B4": -0.0833333,
    "C5": 0.0,
    "C#5": 0.0833333,
    "D5": 0.166667,
    "D#5": 0.25,
    "E5": 0.333333,
    "F5": 0.416667,
    "F#5": 0.5,
    "G5": 0.583333,
    "G#5": 0.666667,
    "A5": 0.75,
    "A#5": 0.833333,
    "B5": 0.916667,
    "C6": 1.0,
}

def get_closest_note_name(note_name):
    if note_name in NoteDictionary:
        return note_name

    sharp_notes = ['C', 'C#', 'D', 'D#', 'E', 'F', 'F#', 'G', 'G#', 'A', 'A#', 'B']
    flat_notes = ['C', 'Db', 'D', 'Eb', 'E', 'F', 'Gb', 'G', 'Ab', 'A', 'Bb', 'B']

    note, octave = note_name[:-1], int(note_name[-1])

    if note in sharp_notes:
        note_index = sharp_notes.index(note)
        for i in range(1, 3):
            for j in [-1, 1]:
                new_index = (note_index + i * j) % 12
                new_note = sharp_notes[new_index] + str(octave)
                if new_note in NoteDictionary:
                    return new_note
    elif note in flat_notes:
        note_index = flat_notes.index(note)
        for i in range(1, 3):
            for j in [-1, 1]:
                new_index = (note_index + i * j) % 12
                new_note = flat_notes[new_index] + str(octave)
                if new_note in NoteDictionary:
                    return new_note

    return note_name



def write_notes_to_file(midi_file_path, output_file_path, short_interval=0.1, long_interval=0.3):
    midi_data = pretty_midi.PrettyMIDI(midi_file_path)
    notes = midi_data.instruments[0].notes
    note_dict = {}
    prev_time = 0
    current_notes = []

    for note in notes:
        note_name = pretty_midi.note_number_to_name(note.pitch)
        # 获取最接近的音符名称
        closest_note_name = get_closest_note_name(note_name)
        if note.start != prev_time:
            if current_notes:
                note_dict[prev_time] = current_notes.copy()
                current_notes.clear()
            prev_time = note.start
        current_notes.append(closest_note_name)

    output_notes = []
    previous_time = None
    for time in sorted(note_dict.keys()):
        if previous_time is not None:
            time_diff = time - previous_time
            if time_diff <= short_interval:
                if output_notes and output_notes[-1] != '\n':
                    output_notes.append(',')  # 在同一行的音符之间用逗号隔开
                output_notes.append(','.join(note_dict[time]))  # 将较短时间内的音符写在同一行
            else:
                num_zeros = int((time_diff - short_interval) // long_interval)
                for _ in range(num_zeros):
                    output_notes.append('\n0')  # 在长时间间隔后换行
                if output_notes and output_notes[-1] != '\n':
                    output_notes.append('\n')  # 在音符之间换行
                output_notes.append(','.join(note_dict[time]))  # 新行写入新的音符
        else:
            output_notes.append(','.join(note_dict[time]))  # 第一个音符不插入0，直接换行

        previous_time = time

    with open(output_file_path, 'w') as f:
        f.write(''.join(output_notes))

# 指定你的 MIDI 文件路径
midi_path = '2：23AM.mid'  # 替换为你自己的 MIDI 文件路径

# 指定输出文件路径
output_path = 'C:\\Users\\xien\\Desktop\\ts\\tshock\\Songs\\2：23AM..txt'  # 替换为你想要输出的文件路径

# 调用函数
write_notes_to_file(midi_path, output_path, short_interval=0.1, long_interval=0.3)
# short_interval表示较短的时间间隔，小于这个值就将两个音符放在同一排，long_interval表示较长的时间间隔，大于这个值就将两个音符之间加0





