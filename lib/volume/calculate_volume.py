import wave
import audioop
import sys

def calculate_volume(wav_file, frame_size, overlap):
    with wave.open(wav_file, 'rb') as wf:
        framerate = wf.getframerate()
        frames = wf.readframes(wf.getnframes())
        
        step = frame_size - overlap
        start = 0
        end = frame_size
        frame_number = 0

        while end <= len(frames):
            frame = frames[start:end]
            volume = audioop.rms(frame, wf.getsampwidth())
            print(f"{frame_number},{volume}")
            
            start += step
            end += step
            frame_number += 1

if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("Usage: python calculate_volume.py <wav_file> <frame_size> <overlap>")
        sys.exit(1)

    wav_file = sys.argv[1]
    frame_size = int(sys.argv[2])
    overlap = int(sys.argv[3])

    calculate_volume(wav_file, frame_size, overlap)
