﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Security;

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
            KeyChain keychain = new KeyChain();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            //Private Key laden und mit Passwort verschlüsseln
            keychain.privateKey = encryptKeySym(byteArrayToString(rsa.ExportCspBlob(true)), privateKeyPassword);
            //Public Key laden
            keychain.publicKey = byteArrayToString(rsa.ExportCspBlob(false));
            //Masterkey generieren und mit publicKey verschlüsseln
            keychain.masterKey = encryptKeyAsym(generateMasterKey(), keychain.publicKey);
            return keychain;

        }

        /// <summary>
        /// Entschlüsselt einen kompletten KeyChain
        /// TODO
        /// </summary>
        /// <param name="fromDatabase"></param>
        /// <returns></returns>
        public static KeyChain decryptKeyChain(KeyChain fromDatabase, String password) {
            KeyChain ret = new KeyChain();
            //Public Key ist nicht verschlüsselt
            ret.publicKey = fromDatabase.publicKey;
            //Private Key ist mit Passwort verschlüsselt
            ret.privateKey = decryptKeySym(fromDatabase.privateKey, password);
            //Masterkey ist mit Public und Privatekey verschlüsselt
            ret.masterKey = decryptKeyAsym(fromDatabase.masterKey, ret.privateKey);
            return ret;
        }

        //Initialisierungsvektor für encryptPrivateKey
        private static readonly byte[] iv = new byte[] { 65, 110, 68, 26, 69, 178, 200, 219 };

        /// <summary>
        /// Verschlüsselt den Privaten Schlüssel mit dem Passwort des privaten Schlüssels
        /// </summary>
        /// <param name="key">Privater Schlüssel</param>
        /// <param name="password">Passwort zum privaten Schlüssel</param>
        /// <returns>Verschlüsselten Key</returns>
        private static String encryptKeySym(String key, String password) {
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
        private static String decryptKeySym(String encryptedKey, String password){
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
            TripleDESCryptoServiceProvider tdes = (TripleDESCryptoServiceProvider)TripleDESCryptoServiceProvider.Create();
            return byteArrayToString(tdes.Key);
        }

        /// <summary>
        /// Verschlüsselt einen Key assymetrisch
        /// </summary>
        /// <param name="key">Schlüssel, der verschlüsselt werden soll</param>
        /// <param name="rsaparamsBLOB">CSP Blob der Parameter (nur public key nötig)</param>
        /// <returns>Verschlüsselter Schlüssel</returns>
        public static String encryptKeyAsym(String key, String rsaparamsBLOB) {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(stringToByteArray(rsaparamsBLOB));
            return byteArrayToString(rsa.Encrypt(stringToByteArray(key), true));  
        }

        /// <summary>
        /// Verschlüsselt einen Key assymetrisch
        /// </summary>
        /// <param name="key">Schlüssel, der verschlüsselt werden soll</param>
        /// <param name="rsaparamsBLOB">CSP Blob der Parameter (nur public key nötig)</param>
        /// <returns>Verschlüsselter Schlüssel</returns>
        public static String decryptKeyAsym(String key, String rsaparamsBLOB)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportCspBlob(stringToByteArray(rsaparamsBLOB));
                return byteArrayToString(rsa.Decrypt(stringToByteArray(key), true));
            }
            catch (Exception e){
                Debug.Write("Fehler beim Entschlüsseln: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Verschlüsselt eine Datei
        /// ! NICHT GETESTET !
        /// </summary>
        /// <param name="inName"></param>
        /// <param name="outName"></param>
        /// <param name="tDesKey"></param>
        public static void encryptFile(Stream inputstream, String filename, KeyChain decryptedKeyChain)
        {
            //Initialisierungsvektor laden
            byte[] tDesIV = iv;

            //Filepfad angeben
            String filepath = "c:\\temp\\";

            //Filestreams für Input- und Output-File erzeugen.
            Stream fin = inputstream;
            FileStream fout = new FileStream(filepath+filename, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Helfervariablen für Lesen und Schreiben.
            byte[] bin = new byte[10000];   //Puffer fürs Verschlüsseln.
            long rdlen = 0;                 //Anzahl der geschriebenen Bytes.
            long totlen = fin.Length;       //Länge der Datei.
            int len;                        //Anzahl der Bytes die auf einmal geschrieben werden sollen.

            TripleDES tDes = new TripleDESCryptoServiceProvider();
            byte[] masterkey = stringToByteArray(decryptedKeyChain.masterKey);
            CryptoStream encStream = new CryptoStream(fout, tDes.CreateEncryptor(masterkey, tDesIV), CryptoStreamMode.Write);

            //Aus dem Input-File lesen, verschlüsseln und in Output-File schreiben
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 10000);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }

            encStream.Close();
            fout.Close();
            fin.Close();

            decryptFile("dasd", "dasdas", decryptedKeyChain);
        }

        /// <summary>
        /// Entschlüsselt eine Datei 
        /// ! NICHT GETESTET !
        /// </summary>
        /// <param name="inName"></param>
        /// <param name="outName"></param>
        /// <param name="tDesKey"></param>
        public static void decryptFile(String inName, String outName, KeyChain decryptedKeyChain)
        {
            byte[] tDesIV = iv;
            //Filepfad angeben
            inName = "c:\\temp\\RHDSetup.log";
            outName = "c:\\temp\\out\\RHDSetup.log";

            //Filestreams für Input- und Output-File erzeugen.
            FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Helfervariablen für Lesen und Schreiben.
            byte[] bin = new byte[10000];   //Puffer fürs Entschlüsseln.
            long rdlen = 0;                 //Anzahl der geschriebenen Bytes
            long totlen = fin.Length;       //Länge der Datei.
            int len;                        //Anzahl der Bytes die auf einmal geschrieben werden sollen.

            TripleDES tDes = new TripleDESCryptoServiceProvider();
            byte[] masterkey = stringToByteArray(decryptedKeyChain.masterKey);
            CryptoStream decStream = new CryptoStream(fout, tDes.CreateDecryptor(masterkey, tDesIV), CryptoStreamMode.Write);

            //Aus dem Input-File lesen, entschlüsseln und in Output-File schreiben
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 10000);
                decStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }

            decStream.Close();
            fout.Close();
            fin.Close();
        }
    }

}