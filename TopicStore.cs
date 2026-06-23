using System;
using System.Collections.Generic;

namespace CyberWareASM
{
    /// <summary>
    /// Extends keyword coverage with 15 additional cybersecurity topics.
    /// Works alongside KeywordResponder to give broad topic coverage.
    /// Each topic maps to a list of responses; one is chosen randomly per call.
    /// </summary>
    public class TopicStore
    {
        private readonly Dictionary<string, List<string>> _topics;
        private readonly Random _rng = new();

        public TopicStore()
        {
            _topics = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                // ── FIREWALL ─────────────────────────────────────────────────
                ["firewall"] = new List<string>
                {
                    "🛡️ A firewall monitors and controls network traffic based on security rules. It is your first barrier between your device and the outside world — keep it enabled!",
                    "🛡️ Home routers have built-in hardware firewalls. Log into your router admin panel and ensure the firewall is active. Change the default admin password while you're there!",
                    "🛡️ Software firewalls (like Windows Defender Firewall) complement hardware firewalls by filtering per-application traffic. Use both layers for stronger protection."
                },

                // ── VPN ───────────────────────────────────────────────────────
                ["vpn"] = new List<string>
                {
                    "🌐 A VPN (Virtual Private Network) encrypts your internet connection, hiding your activity from your ISP and attackers on the same network.",
                    "🌐 Always use a VPN on public Wi-Fi at cafes, airports and hotels. Without one, anyone on the same network can intercept your unencrypted traffic.",
                    "🌐 Choose a reputable VPN with a strict no-log policy — ProtonVPN and Mullvad are well audited. Free VPNs often monetise your data instead of protecting it."
                },

                // ── ENCRYPTION ───────────────────────────────────────────────
                ["encryption"] = new List<string>
                {
                    "🔑 Encryption converts your data into an unreadable format that only authorised parties can decode. Without the key, stolen data is useless to an attacker.",
                    "🔑 End-to-end encryption (E2EE) means only you and your recipient can read messages — not even the service provider. Use Signal or WhatsApp for sensitive conversations.",
                    "🔑 Enable full-disk encryption on all devices. Windows offers BitLocker; macOS has FileVault. If your laptop is stolen, your data stays private."
                },

                // ── HACKING ───────────────────────────────────────────────────
                ["hacking"] = new List<string>
                {
                    "💻 Ethical hackers (penetration testers) are hired to break into systems legally to find vulnerabilities before criminals do. It's a thriving and well-paid career path!",
                    "💻 Common hacking techniques include SQL injection, cross-site scripting (XSS), and credential stuffing. Understanding them is the first step to defending against them.",
                    "💻 Patch your software! Most successful attacks exploit vulnerabilities that had available fixes for months. Timely updates are the single highest-impact security habit."
                },

                // ── TWO-FACTOR AUTH ───────────────────────────────────────────
                ["2fa"] = new List<string>
                {
                    "📱 Two-factor authentication (2FA) adds a second verification step on top of your password. Even if your password leaks, attackers still can't log in.",
                    "📱 Use authenticator apps like Google Authenticator or Authy instead of SMS-based 2FA — SMS codes can be intercepted via SIM-swapping attacks.",
                    "📱 Enable 2FA on every critical account: email, banking, cloud storage and social media. It takes under a minute and dramatically reduces your risk."
                },

                // ── SOCIAL ENGINEERING ────────────────────────────────────────
                ["social engineering"] = new List<string>
                {
                    "🎭 Social engineering manipulates people psychologically to reveal confidential information rather than hacking the technology. Always verify identities through a separate channel.",
                    "🎭 Pretexting (making up a scenario), baiting (USB drops) and tailgating (following someone through a secure door) are real social engineering tactics used daily.",
                    "🎭 Regular security awareness training is the best defence. A technically perfect firewall is useless if an employee gives away their password on the phone."
                },

                // ── RANSOMWARE ────────────────────────────────────────────────
                ["ransomware"] = new List<string>
                {
                    "💀 Ransomware encrypts your entire file system and demands cryptocurrency payment to restore access. Offline backups are your only guaranteed recovery method.",
                    "💀 Never pay the ransom — there is no guarantee of file recovery and you directly fund criminal operations. Report the incident to local cybercrime authorities.",
                    "💀 The 3-2-1 backup rule: keep 3 copies of your data, on 2 different media types, with 1 copy offsite or offline. Ransomware cannot encrypt what it cannot reach."
                },

                // ── DATA BREACH ───────────────────────────────────────────────
                ["data breach"] = new List<string>
                {
                    "📊 A data breach exposes confidential information. Visit haveibeenpwned.com now to check whether your email address has been in any known breaches.",
                    "📊 If notified of a breach, change that site's password immediately and any other accounts where you reused it. Enable 2FA on all affected accounts.",
                    "📊 Minimise your exposure by providing only essential information to online services. The less data they hold, the less you lose if they are breached."
                },

                // ── ANTIVIRUS ─────────────────────────────────────────────────
                ["antivirus"] = new List<string>
                {
                    "🛡️ Antivirus software detects and neutralises known malware. Keep definitions updated daily — new threats are discovered constantly.",
                    "🛡️ Windows Defender (built into Windows 10/11) is a solid baseline. Supplementing with Malwarebytes' free scanner adds an extra detection layer.",
                    "🛡️ Never disable your antivirus, even if an app or installer tells you to. That is a classic social engineering trick used by malware installers."
                },

                // ── DARK WEB ──────────────────────────────────────────────────
                ["dark web"] = new List<string>
                {
                    "🌑 The dark web is an encrypted part of the internet often used for illegal activities including selling stolen credentials, credit cards and personal data.",
                    "🌑 Your data may already be for sale on dark web marketplaces after a breach. Dark web monitoring services (e.g. those built into password managers) alert you instantly.",
                    "🌑 Avoid the dark web unless you are a security professional. The risks — malware, scams and legal exposure — vastly outweigh any curiosity."
                },

                // ── CLOUD SECURITY ────────────────────────────────────────────
                ["cloud security"] = new List<string>
                {
                    "☁️ Cloud security is a shared responsibility. The provider secures the infrastructure; YOU are responsible for access controls, data classification and account security.",
                    "☁️ Review sharing settings on cloud files regularly. Accidentally setting a sensitive document to 'Anyone with the link can view' is one of the most common cloud mistakes.",
                    "☁️ Enable multi-factor authentication on all cloud accounts (Google Drive, OneDrive, Dropbox). A single compromised password should never be enough to access your data."
                },

                // ── NETWORK SECURITY ──────────────────────────────────────────
                ["network security"] = new List<string>
                {
                    "🔌 Secure your home Wi-Fi by changing the default router credentials and using WPA3 (or WPA2 minimum) encryption. Avoid WEP — it is trivially crackable.",
                    "🔌 Create a separate guest network for IoT devices (smart TVs, cameras, doorbells). Keeping them isolated protects your main devices if an IoT device is compromised.",
                    "🔌 Regularly audit which devices are connected to your Wi-Fi from your router's admin panel. Any unfamiliar device could indicate an intrusion."
                },

                // ── APPLICATION SECURITY ──────────────────────────────────────
                ["application security"] = new List<string>
                {
                    "🔧 Application security focuses on identifying and fixing vulnerabilities during software development, not after deployment. 'Shift left' — build security in from day one.",
                    "🔧 OWASP's Top 10 lists the most critical web application security risks. SQL injection and broken authentication consistently rank at the top — know these if you code.",
                    "🔧 Always validate and sanitise user input on the server side. Never trust client-side validation alone — it can be bypassed with browser developer tools."
                },

                // ── ENDPOINT SECURITY ─────────────────────────────────────────
                ["endpoint security"] = new List<string>
                {
                    "💻 Endpoint security protects every device (laptop, phone, tablet) that connects to your network. A single unpatched device can be the entry point for a whole network breach.",
                    "💻 Mobile Device Management (MDM) software lets organisations remotely wipe stolen devices. Enable remote wipe on your personal phone through your OS settings.",
                    "💻 Lock your devices with a strong PIN, password or biometrics. An unlocked, unattended device is an open door for data theft."
                },

                // ── THREAT INTELLIGENCE ───────────────────────────────────────
                ["threat intelligence"] = new List<string>
                {
                    "🔍 Threat intelligence is information about active threats that helps organisations defend proactively. Think of it as a neighbourhood watch — but for cyberattacks.",
                    "🔍 Free sources like CISA advisories, Krebs on Security and the SANS Internet Storm Centre publish real-time threat intelligence accessible to everyone.",
                    "🔍 Sharing threat intelligence across organisations (called 'information sharing') is how the cybersecurity community collectively fights criminal groups."
                },

                // ── INCIDENT RESPONSE ─────────────────────────────────────────
                ["incident response"] = new List<string>
                {
                    "🚨 Incident Response (IR) is the structured process of handling a cyberattack: Preparation → Detection → Containment → Eradication → Recovery → Lessons Learned.",
                    "🚨 The faster you detect and contain a breach, the less damage it causes. Average detection time is still over 200 days in many organisations — that is 200 days of data theft.",
                    "🚨 If you suspect your device is compromised: disconnect from the internet immediately, don't turn off the machine (evidence lives in RAM), and contact a security professional."
                },

                // ── CYBER AWARENESS ───────────────────────────────────────────
                ["cyber awareness"] = new List<string>
                {
                    "📢 Cyber awareness means understanding everyday digital risks and behaving accordingly. It is the human layer of security — and often the weakest link.",
                    "📢 95% of successful cyberattacks involve human error at some point. Regular training — even watching 10-minute awareness videos — dramatically reduces that risk.",
                    "📢 Share what you learn! Teaching a family member or colleague about phishing or strong passwords multiplies the protective effect of cybersecurity awareness."
                },

                // ── GOVERNANCE & COMPLIANCE ───────────────────────────────────
                ["governance"] = new List<string>
                {
                    "📋 Cybersecurity governance is the framework of policies and accountability structures that guide how an organisation manages digital risk.",
                    "📋 Regulations like GDPR (Europe), POPIA (South Africa) and HIPAA (USA healthcare) create legal obligations around data protection. Non-compliance can mean massive fines.",
                    "📋 A good governance framework starts with knowing what data you hold, classifying it by sensitivity, and applying proportional controls to the most critical assets."
                },

                // ── CRYPTOGRAPHY ──────────────────────────────────────────────
                ["cryptography"] = new List<string>
                {
                    "🔐 Cryptography is the science of securing communication using mathematical algorithms. Modern HTTPS uses TLS cryptography to protect every secure website you visit.",
                    "🔐 Symmetric encryption (one key for both encrypt/decrypt) is fast — used for bulk data. Asymmetric encryption (public + private key pair) is used for key exchange and signatures.",
                    "🔐 Quantum computing threatens current RSA and ECC cryptography. Researchers are already standardising post-quantum cryptographic algorithms to stay ahead of the threat."
                }
            };
        }

        /// <summary>
        /// Scans the input for a known topic keyword. Returns a random matching
        /// response, or null if no topic is matched.
        /// </summary>
        public string? GetResponse(string input)
        {
            string lower = input.ToLowerInvariant();
            foreach (var kvp in _topics)
                if (lower.Contains(kvp.Key.ToLowerInvariant()))
                    return kvp.Value[_rng.Next(kvp.Value.Count)];

            return null;
        }

        /// <summary>Returns a list of all recognised topic strings.</summary>
        public List<string> GetAllTopics() => new(_topics.Keys);
    }
}
