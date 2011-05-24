using System;
using System.Security.Cryptography;
using System.Text;

namespace SIT
{

    public static class SecurityProvider
    {

        /// <summary>
        /// Hasht ein Passwort mit dem Secure Hash Algorithm (Länge 256)
        /// GETESTET und funktioniert
        /// </summary>
        /// <param name="pass">Passwort das zu Hashen ist</param>
        /// <returns>gehashtes Passwort</returns>
        public static String hashPassword(String pass) {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(pass));
            return byteArrayToString(hashedDataBytes);
        }

        /// <summary>
        /// Verrgleicht ein Passwort, ob es mit einem Hash übereinstimmt
        /// GETESTET und funktioniert
        /// </summary>
        /// <param name="pass">Passwort</param>
        /// <param name="hash">Passwort als Hash</param>s
        /// <returns>Übereinstimmung</returns>
        public static Boolean compareHash(String pass, String hash) {
            return (hashPassword(pass).Equals(hash));
        }

        /// <summary>
        /// Wandelt ein Byte-Array in einen String um
        /// </summary>
        /// <param name="inputArray">Byte-Array, das zu einem String umgewandelt werden soll</param>
        /// <returns>String des Bytearrays</returns>
        private static string byteArrayToString(byte[] inputArray)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetString(inputArray);
        }

        /// <summary>
        /// Wandelt einen String in ein Byte-Array um
        /// </summary>
        /// <param name="input">String der zu einem Byte-Array umgewandelt werden soll</param>
        /// <returns></returns>
        private static byte[] stringToByteArray(String input)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(input);
        }

        /// <summary>
        /// Generiert einen neuen Schlüsselbund für den Benutzer
        /// </summary>
        /// <param name="privateKeyPassword">Das Passwort, mit dem der private Schlüssel verschlüsselt wird</param>
        /// <returns></returns>
        public static KeyChain createKeys(String privateKeyPassword)
        {
            KeyChain keys = new KeyChain();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            //Private Key laden
            byte[] privateKey = rsa.ExportCspBlob(true);
            //TODO Private Key mit Passwort verschlüsseln

            //Public Key ladens
            String publicKey = byteArrayToString(rsa.ExportCspBlob(false));
            //TODO Masterkey generieren und mit publicKey verschlüsseln

            return keys;
        
        }

        /// <summary>
        /// Verschlüsselt den Privaten Schlüssel mit dem Passwort des privaten Schlüssels
        /// </summary>
        /// <param name="key">Privater Schlüssel</param>
        /// <param name="password">Passwort zum privaten Schlüssel</param>
        /// <returns></returns>
        private static String encryptPrivateKey(String key, String password) {
            TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider();
            TDES.Key = ASCIIEncoding.ASCII.GetBytes(password);
            TDES.IV = ASCIIEncoding.ASCII.GetBytes(password);
            ICryptoTransform tdesencrypt = TDES.CreateEncryptor();
            //CryptoStream cryptostream = new CryptoStream(fsEncrypted,
            //   desencrypt,
            //   CryptoStreamMode.Write)
            return "";
        }

        /// <summary>
        /// Generiert den Masterschlüssel zur Dateiverschlüsselung
        /// </summary>
        /// <returns></returns>
        private static String generateMasterKey() {
            // Erzeuge eine Instanz eines TripleDES. Key und Vektor werden automatisch generiert.
            TripleDESCryptoServiceProvider tdesCrypto = (TripleDESCryptoServiceProvider)TripleDESCryptoServiceProvider.Create();
            // Gebe den Key als String zurück
            return ASCIIEncoding.ASCII.GetString(tdesCrypto.Key);
        }
    }

}