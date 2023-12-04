document.getElementById('fileInput').addEventListener('change', function (event) {
  const file = event.target.files[0];

  if (file) {
      const reader = new FileReader();

      reader.onload = function (e) {
          const audioContext = new (window.AudioContext || window.webkitAudioContext)();
          const audioBuffer = decodeAudioData(audioContext, e.target.result);

          audioBuffer.then(buffer => {
              const frameSize = 2048;
              const overlap = 512;

              // Convert the buffer to Float32Array
              const float32Array = new Float32Array(buffer.numberOfChannels * buffer.length);
              for (let channel = 0; channel < buffer.numberOfChannels; channel++) {
                  float32Array.set(buffer.getChannelData(channel), channel * buffer.length);
              }

              const step = frameSize - overlap;
              let start = 0;
              let end = frameSize;
              let frameNumber = 0;

              while (end <= float32Array.length) {
                  const frame = float32Array.subarray(start, end);
                  const volume = calculateRMS(frame);
                  console.log(`${frameNumber},${volume}`);

                  start += step;
                  end += step;
                  frameNumber++;
              }
          }).catch(error => {
              console.error('Error decoding audio data:', error);
          });
      };

      reader.readAsArrayBuffer(file);
  }
});

function decodeAudioData(audioContext, arrayBuffer) {
  return new Promise((resolve, reject) => {
      audioContext.decodeAudioData(arrayBuffer, resolve, reject);
  });
}

function calculateRMS(frame) {
  let squareSum = 0;

  for (let i = 0; i < frame.length; i++) {
      squareSum += frame[i] ** 2;
  }

  const rms = Math.sqrt(squareSum / frame.length);
  return rms;
}
