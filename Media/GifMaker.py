from moviepy.editor import VideoFileClip

clip = VideoFileClip("Media/TinyShootersCompilation.mp4")
clip.write_gif("Media/TinyShootersCompilation.gif", 5)