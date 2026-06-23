using System;
using System.IO;
using System.Media;

namespace CyberWareASM
{
    
    // Handles all audio playback for the CyberWare chatbot.
    // Place Ahlumile.wav in the same folder as the .exe (bin/Debug/net10.0).
    public class AudioPlayer
    {
        
        // Plays a .wav file by filename. Looks in the executable's directory.
        public void PlaySound(string fileName)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(basePath, fileName);

                if (File.Exists(fullPath))
                {
                    SoundPlayer player = new SoundPlayer(fullPath);
                    player.Play(); // non-blocking
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[AudioPlayer] File not found: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AudioPlayer] Error: {ex.Message}");
            }
        }

        
        // Plays the greeting sound: Ahlumile.wav
        public void PlayGreeting() => PlaySound("Ahlumile.wav");
    }
}
