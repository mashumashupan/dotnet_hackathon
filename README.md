# Generating watercolors from music

## Overview
This AI application is developed in .NET 8. It uses AI to output a polka dot pattern from the pitch, volume, and time information of an input music file. By inputting the polka dot pattern for each note into the image-to-image AI, the pattern is converted into a real-time watercolor painting and output, finally producing a single picture.
https://docs.google.com/presentation/d/16v9NDRWUQ8lo2LzzX0b2dvCAizPgBR8tJiub6C3PnQ4/edit?usp=sharing

## Prerequisites
Before running the application, ensure that you have the following prerequisites installed:
.NET 8 SDK:[Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)

## Usage
Run the application:
```dotnet run```

## Application Results
![IMG_3880](https://github.com/mashumashupan/dotnet_hackathon/assets/103844832/6ec54310-ac2b-4f98-b878-185a4a6ba2dc)

## Output
1. A music file is input and sound information (pitch, loudness, and time) is received and put into the AI as input.

2. Based on the sound information, polka dots are output in accordance with the music, with the color corresponding to the pitch and the size of the circle corresponding to the volume of the sound.

3. For each additional note, an image is output and input to the image to image AI.

4. The image to image AI outputs the polka dots in a watercolor style in real time.

5. An animation is played in which the sound is drawn as a picture along with the music, and finally the song is output as a single picture.
