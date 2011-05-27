using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;

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
        /// GETESTET und funktioniert
        /// </summary>
        /// <param name="inputArray">Byte-Array, das zu einem String umgewandelt werden soll</param>
        /// <returns>String des Bytearrays</returns>
        private static string byteArrayToString(byte[] inputArray)
        {
            //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            //return encoding.GetString(inputArray);
            return System.Text.Encoding.Default.GetString(inputArray);
        }

        /// <summary>
        /// Wandelt einen String in ein Byte-Array um
        /// GETESTET und funktioniert
        /// </summary>
        /// <param name="input">String der zu einem Byte-Array umgewandelt werden soll</param>
        /// <returns></returns>
        private static byte[] stringToByteArray(String input)
        {
            //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            //return encoding.GetBytes(input);
            return System.Text.Encoding.Default.GetBytes(input);
        }

        /// <summary>
        /// Generiert einen neuen Schlüsselbund für den Benutzer
        /// </summary>
        /// <param name="privateKeyPassword">Das Passwort, mit dem der private Schlüssel verschlüsselt wird</param>
        /// <returns></returns>
        public static KeyChain createKeys(String privateKeyPassword)
        {
            String enc = encryptPrivateKey("text","passwort");
            String dec = decryptPrivateKey(enc, "passwort");

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

        //Initialisierungsvektor für encryptPrivateKey
        private static readonly byte[] iv = new byte[] { 65, 110, 68, 26, 69, 178, 200, 219 };

        /// <summary>
        /// Verschlüsselt den Privaten Schlüssel mit dem Passwort des privaten Schlüssels
        /// </summary>
        /// <param name="key">Privater Schlüssel</param>
        /// <param name="password">Passwort zum privaten Schlüssel</param>
        /// <returns>Verschlüsselten Key</returns>
        private static String encryptPrivateKey(String key, String password) {
            {
                try
                {
                    // MemoryStream Objekt erzeugen
                    MemoryStream memoryStream = new MemoryStream();

                    //Passwort hashen
                    MD5 md5Hash = new MD5CryptoServiceProvider();
                    byte[] hash = md5Hash.ComputeHash(stringToByteArray(password));

                    // Eingabestring in ein Byte-Array konvertieren
                    byte[] toEncrypt = stringToByteArray(key);

                    // CryptoStream Objekt erzeugen und den Initialisierungs-Vektor
                    // sowie den Schlüssel übergeben.
                    CryptoStream cryptoStream = new CryptoStream(
                    memoryStream, new TripleDESCryptoServiceProvider().CreateEncryptor(hash, iv), CryptoStreamMode.Write);

                    // Byte-Array in den Stream schreiben und flushen.
                    cryptoStream.Write(toEncrypt, 0, toEncrypt.Length);
                    cryptoStream.FlushFinalBlock();

                    // Ein Byte-Array aus dem Memory-Stream auslesen
                    byte[] ret = memoryStream.ToArray();

                    // Stream schließen.
                    cryptoStream.Close();
                    memoryStream.Close();

                    // Rückgabewert.
                    return byteArrayToString(ret);
                }
                catch (CryptographicException e)
                {
                    Debug.Write("Fehler beim Verschlüsseln: {0}", e.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Entschlüsselt einen Key
        /// </summary>
        /// <param name="encryptedKey">Verschlüsselter Key</param>
        /// <param name="password">Passwort zum entpacken</param>
        /// <returns>Entschlüsselter Key</returns>
        private static String decryptPrivateKey(String encryptedKey, String password){
            try
            {
                byte[] keyAsByte = stringToByteArray(encryptedKey);
                // Ein MemoryStream Objekt erzeugen und das Byte-Array
                // mit den verschlüsselten Daten zuweisen.
                MemoryStream memoryStream = new MemoryStream(keyAsByte);

                //Passwort hashen
                MD5 md5Hash = new MD5CryptoServiceProvider();
                byte[] hash = md5Hash.ComputeHash(stringToByteArray(password));

                // Ein CryptoStream Objekt erzeugen und den MemoryStream hinzufügen.
                // Den Schlüssel und Initialisierungsvektor zum entschlüsseln verwenden.
                CryptoStream cryptoStream = new CryptoStream(
                memoryStream,
                new TripleDESCryptoServiceProvider().CreateDecryptor(hash, iv), CryptoStreamMode.Read);
                // Buffer erstellen um die entschlüsselten Daten zuzuweisen.
                byte[] fromEncrypt = new byte[keyAsByte.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                // Die entschlüsselten Daten aus dem CryptoStream lesen
                // und im temporären Puffer ablegen.
                cryptoStream.Read(fromEncrypt, 0, fromEncrypt.Length);

                // Bytearray untersuchen
                int i = fromEncrypt.Length - 1;
                while (fromEncrypt[i] == 0)
                    --i;
                // fromEncrypt[i] ist das letzte Byte (nicht 0)
                byte[] ret = new byte[i + 1];
                Array.Copy(fromEncrypt, ret, i + 1);

                // Den Puffer in einen String konvertieren und zurückgeben.
                return byteArrayToString(ret);
            }
            catch (CryptographicException e)
            {
                Debug.Write("Fehler beim Entschlüsseln: {0}", e.Message);
                return null;
            }
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