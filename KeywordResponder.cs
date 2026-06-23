using System;
using System.Collections.Generic;

namespace CyberWareASM
{
  
    public class KeywordResponder
    {
        private readonly Dictionary<string, List<string>> _responses;
        private readonly Random _rng = new();

        public KeywordResponder()
        {
            _responses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                // PHISHING
                ["phishing"] = new List<string>
                {
                    "🎣 Phishing attacks trick you into revealing sensitive information by disguising as trusted sources. Always verify the sender's full email address before clicking any link.",
                    "🎣 Phishing emails create a false sense of urgency — 'Your account will be closed in 24 hours!' Slow down, take a breath, and verify through official channels.",
                    "🎣 Watch for red flags: misspelled domains (amaz0n.com), generic greetings ('Dear Customer'), and unexpected attachments. When in doubt — don't click!",
                    "🎣 Enable anti-phishing protection in your browser and email client. Tools like Google Safe Browsing flag malicious sites automatically.",
                    "🎣 Spear-phishing targets YOU specifically using your personal information. Never assume an email is safe just because it mentions your name or company."
                },

                // PASSWORD
                ["password"] = new List<string>
                {
                    "🔐 Strong passwords are at least 14 characters long, mixing upper/lowercase letters, numbers and symbols. Avoid dictionary words and personal info like your birthdate.",
                    "🔐 Never reuse passwords across accounts! If one site is breached, attackers will try those credentials everywhere. Use a password manager like Bitwarden or 1Password.",
                    "🔐 Pair strong passwords with two-factor authentication (2FA). Even if your password leaks, attackers still can't get in without your second factor.",
                    "🔐 Change passwords immediately after any breach. Check haveibeenpwned.com to see if your email has been in known data dumps.",
                    "🔐 Password managers generate and store complex, unique passwords for every account. You only need to remember one master password — make it strong!"
                },

                // PRIVACY
                ["privacy"] = new List<string>
                {
                    "🔒 Review your app permissions regularly. Does that photo-editing app really need your microphone? Revoke permissions that don't match an app's core purpose.",
                    "🔒 Use a VPN (Virtual Private Network) on public Wi-Fi to encrypt your internet traffic and shield your activity from network eavesdroppers.",
                    "🔒 Tighten your social media privacy settings. Limit who can see your posts, your friend list and your location history.",
                    "🔒 Use privacy-focused browsers like Brave or Firefox with the uBlock Origin extension to block trackers and reduce your digital footprint.",
                    "🔒 Read privacy policies (or use tools like Tosdr.org that summarise them). Knowing what data you hand over is the first step to protecting it."
                },

                // SCAM
                ["scam"] = new List<string>
                {
                    "⚠️ If an offer sounds too good to be true, it almost certainly is. Lottery wins you never entered, unexpected inheritances and 'free' gift cards are classic scam bait.",
                    "⚠️ Tech-support scams often call or pop up claiming your PC is infected. Legitimate companies like Microsoft will NEVER cold-call you about viruses.",
                    "⚠️ Romance scams build emotional trust over weeks before requesting money. Never send money to someone you haven't verified in person.",
                    "⚠️ Report scams to your national cybercrime authority (e.g. the SAPS Cybercrime Unit in South Africa). Your report helps protect others.",
                    "⚠️ QR code scams are rising — attackers replace legitimate QR codes with malicious ones. Always check the URL a QR code resolves to before proceeding."
                },

                // MALWARE
                ["malware"] = new List<string>
                {
                    "🦠 Malware is malicious software designed to damage systems or steal data. Keep your OS, apps and antivirus definitions updated to close known exploits.",
                    "🦠 Only download software from official websites and verified app stores. Cracked software and pirated games are the #1 malware delivery method.",
                    "🦠 Ransomware encrypts all your files and demands payment to restore them. Your best defence is regular, offline backups — and never paying the ransom.",
                    "🦠 Signs your device may be infected: unexpected slowdowns, pop-up ads, programs launching themselves, or your browser redirecting. Run a full scan immediately!",
                    "🦠 Fileless malware lives in your computer's memory (RAM) rather than on disk, making it invisible to traditional scanners. Keep memory-scanning behaviour-based AV active."
                }
            };
        }

        /// <summary>
        /// Scans the input for a known keyword. If found, returns a randomly
        /// selected response from that keyword's list. Returns null if no match.
        /// </summary>
        public string? GetResponse(string input)
        {
            string lower = input.ToLowerInvariant();
            foreach (var kvp in _responses)
                if (lower.Contains(kvp.Key.ToLowerInvariant()))
                    return kvp.Value[_rng.Next(kvp.Value.Count)];

            return null;
        }

        /// <summary>Returns a list of all recognised keyword strings.</summary>
        public List<string> GetAllKeywords() => new(_responses.Keys);
    }
}
