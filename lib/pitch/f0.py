import numpy as np
import librosa
import matplotlib.pyplot as plt

# ファイルパスを直接指定
filepath = "./piano.wav"

np.set_printoptions(threshold=np.inf)
sr = 16000


# librosaを使用して音声ファイルを読み込む
y, sr = librosa.load(filepath, sr=sr)
fmin, fmax = 110, 660

print(5)

# librosa.pyinを使用して基本周波数（f0）を推定
fo_pyin, voiced_flag, voiced_prob = librosa.pyin(y, fmin, fmax)

print(6)

# 結果をtxtファイルに書き込む
output_file_path = "./b.txt"

# ファイルを開いて書き込む
with open(output_file_path, 'w') as output_file:
    # ファイルに結果を書き込む
    output_file.write(np.array2string(fo_pyin))
