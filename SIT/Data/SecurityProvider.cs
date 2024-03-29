﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Security;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI;
using System.Threading;

namespace SIT
{

    public static class SecurityProvider
    {

        //Masterpasswort
        private static String masterpw = "SIT2011MASTER123456789fsdfhrqdfoiq3rzihbqirsadfbvciurw";

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
        /// Erzeugt einen Neuen Masterkey für den Benutzer
        /// </summary>
        /// <param name="decryptedKeyChain">Entschlüsselter Schlüsselbund</param>
        /// <param name="SelectAllMasterKeys">SelectAllMasterKeys-Verbindung</param>
        public static void createNewMasterKey(KeyChain decryptedKeyChain, SqlDataSource SelectAllMasterKeys) {
            String newMasterkey = encryptKeyAsym(generateMasterKey(), decryptedKeyChain.publicKey);
            SelectAllMasterKeys.InsertParameters["MasterKey"].DefaultValue = newMasterkey;
            SelectAllMasterKeys.Insert();
        }

        /// <summary>
        /// Entschlüsselt einen kompletten KeyChain
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
            //ID vom Eingangsschlüsselbund übernehmen
            ret.masterKeyID = fromDatabase.masterKeyID;
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

                //Passwort als MD5 Hashen, weil MD5 als Schlüssel für CreateDecryptor aktzeptiert wird
                MD5 md5Hash = new MD5CryptoServiceProvider();
                byte[] hash = md5Hash.ComputeHash(stringToByteArray(password));

                // Ein CryptoStream Objekt erzeugen und den MemoryStream hinzufügen.
                // Den Schlüssel und Initialisierungsvektor zum entschlüsseln verwenden.
                CryptoStream cryptoStream = new CryptoStream(
                memoryStream,
                new TripleDESCryptoServiceProvider().CreateDecryptor(hash, iv), CryptoStreamMode.Read);
                // Buffer erstellen um die entschlüsselten Daten zuzuweisen.
                byte[] fromEncrypt = new byte[keyAsByte.Length];

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
        /// Verschlüsselt eine Datei und erstellt einen Header
        /// </summary>
        /// <param name="inputstream">Uploadstream</param>
        /// <param name="filename">Dateiname</param>
        /// <param name="decryptedKeyChain">Entschlüsselter Schlüsselbund</param>
        public static String encryptFile(Stream inputstream, String filename, KeyChain decryptedKeyChain)
        {
            //Initialisierungsvektor laden
            byte[] tDesIV = iv;

            //Filepfad angeben
            String filepath = SIT_Ressources.EncryptedPath;

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

            //Header schreiben und Schlüssel mit Masterpasswort verschlüsseln
            byte[] header = stringToByteArray(
                "[KeyID]"+
                decryptedKeyChain.masterKeyID+
                "[Public]" +
                decryptedKeyChain.publicKey+
                "[Private]" +
                encryptKeySym(decryptedKeyChain.privateKey,masterpw)+
                "[Master]" +
                encryptKeyAsym(decryptedKeyChain.masterKey,decryptedKeyChain.publicKey)+
                "[Data]"
                );
            fout.Write(header, 0, header.Length);

            //Aus dem Input-File lesen, verschlüsseln und in Output-File schreiben
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 10000);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }

            //Verschlüsselungsstream, Eingangsstream und Ausgangsstream schließen
            encStream.Close();
            fout.Close();
            fin.Close();

            //Pfad der verschlüsselten Datei zurückgeben
            return filepath + filename;
        }

        /// <summary>
        /// Entschlüsselt eine Datei 
        /// </summary>
        /// <param name="inName"></param>
        /// <param name="outName"></param>
        /// <param name="tDesKey"></param>
        public static String decryptFile(String filename, SqlDataSource MasterKeySelect, KeyChain decryptedKeyChain)
        {

            //Initialisierungsvektor laden
            byte[] tDesIV = iv;

            //Filestreams für Input- und Output-File erzeugen.
            FileStream fin1 = new FileStream(SIT_Ressources.EncryptedPath + filename, FileMode.Open, FileAccess.Read);

            //Header laden
            LinkedList<String> headerList = new LinkedList<String>();
            byte[] headerByte = new byte[1000];
            int length = 0;
            
            //Headerlänge ermitteln
            bool DataTagFound = false;
            while (!DataTagFound && length < fin1.Length) {
                //Zeichen lesen
                length += fin1.Read(headerByte, 0, 1000);
                //Zeichen zwischenspeichern
                headerList.AddLast(byteArrayToString(headerByte));
                //Aktuellen String prüfen, ob der Datenblock erreicht wurde
                if (headerList.Last.Value.Contains("[Data]"))
                {
                    DataTagFound = true;
                }
                else {
                    //Position um 500 Zeichen nach vorne verlegen um String richtig zu lesen
                    fin1.Position -= 500;
                }
            }
            //Ersten Filestream schließen
            fin1.Close();

            //Wenn der Header richtig gesetzt wurde
            if (DataTagFound)
            {
                //Header erstellen
                StringBuilder headerBuilder = new StringBuilder();
                //Zähler um nur gerade Indizes zum Stringbuilder hinzuzufügen
                int i = 0;
                foreach (String current in headerList)
                {
                    //Wenn gerader Index, füge zum Stringbuilder hinzu
                    if (i % 2 == 0)
                    {
                        headerBuilder.Append(current);
                    }
                    i++;
                }

                //Filestream für Input-File erzeugen frisch erzeugen
                Stream fin2 = new FileStream(SIT_Ressources.EncryptedPath + filename, FileMode.Open, FileAccess.Read);

                //Header als String laden
                String header = headerBuilder.ToString();
                //String auf nötiges reduzieren
                Regex headerRegex = new Regex("\\[KeyID](.|\n)*\\[Public](.|\n)*\\[Private](.|\n)*\\[Master](.|\n)*\\[Data]");
                //Header zu Regex matchen
                Match headerMatch = headerRegex.Match(header);
                header = headerMatch.Value;

                //Wenn kein Ergebnis vom Regulären Ausdruck, dann wurde die Syntax der Datei geändert -> Abbruch
                if (header != null)
                {
                    //Erzeuge Schlüsselbund aus Headerstring
                    KeyChain headerKeyChain = getKeyChainFromHeader(header);

                    //Prüfe ob der Benutzer ein Besitzer des Masterkeys ist (Kopie oder Original)
                    MasterKeySelect.SelectParameters["KeyID"].DefaultValue = headerKeyChain.masterKeyID;
                    DataView record = (DataView)MasterKeySelect.Select(DataSourceSelectArguments.Empty);

                    //Benutzer ist im Besitz des Schlüssels
                    if (record.Count != 0)
                    {
                        //Masterkey aus Tabelle holen
                        String masterkey = record[0]["MasterKey"].ToString();
                        //Masterkey aus Tabelle mit eigenem PrivateKey entschlüsseln
                        masterkey = decryptKeyAsym(masterkey, decryptedKeyChain.privateKey);

                        //Header als Byte-Array speichern
                        headerByte = stringToByteArray(header);

                        //Header im Stream weglesen (wird nicht entschlüsselt)
                        fin2.Position = headerByte.Length;

                        //Outputstream anlegen
                        FileStream fout = new FileStream(SIT_Ressources.DecryptedPath+filename, FileMode.OpenOrCreate, FileAccess.Write);
                        fout.SetLength(0);

                        //Helfervariablen für Lesen und Schreiben.
                        byte[] bin = new byte[10000];   //Puffer fürs Entschlüsseln.
                        long rdlen = 0;                 //Anzahl der geschriebenen Bytes
                        long totlen = fin2.Length - headerByte.Length;       //Länge der Datei ohne Header
                        int len;                        //Anzahl der Bytes die auf einmal geschrieben werden sollen.

                        TripleDES tDes = new TripleDESCryptoServiceProvider();
                        byte[] masterkeyBit = stringToByteArray(masterkey);
                        //Masterkey überschreiben
                        masterkey = "";
                        CryptoStream decStream = new CryptoStream(fout, tDes.CreateDecryptor(masterkeyBit, tDesIV), CryptoStreamMode.Write);

                        //Aus dem Input-File lesen, entschlüsseln und in Output-File schreiben
                        while (rdlen < totlen)
                        {
                            len = fin2.Read(bin, 0, 10000);
                            decStream.Write(bin, 0, len);
                            rdlen = rdlen + len;
                        }

                        //Alles Schließen
                        decStream.Close();
                        fout.Close();
                        fin2.Close();

                        //Pfad der entschlüsselten Datei zurückgeben
                        return SIT_Ressources.DecryptedPath + filename;

                    }
                    else {
                        //Offenen Stream schließen
                        fin2.Close();
                        throw new Exception("Die Datei kann nicht entschlüsselt werden (Sie sind nicht im Besitz des passenden Schlüssels)");
                    }
                }
                else {
                    //Offenen Stream schließen
                    fin2.Close();
                    throw new Exception("Die Datei kann nicht entschlüsselt werden, da sie nicht im gültigen Format vorliegt: Header fehlerhaft.");
                }
            }
            else {
                //Header konnte nicht gelesen werden
                throw new Exception("Die Datei kann nicht entschlüsselt werden, da sie nicht im gültigen Format vorliegt: Data-Tag nicht gefunden.");
            }
        }

        /// <summary>
        /// Liest aus einer Headerdatei einen Schlüsselbund aus
        /// </summary>
        /// <param name="header">Header, der von der Datei abgetrennt ist</param>
        /// <returns>Schlüsselbund aus einem Dateiheader</returns>
        private static KeyChain getKeyChainFromHeader(String header) {
            Regex extractInfo = new Regex(@"\[KeyID](?<KeyID>(.|\n)*)\[Public](?<PublicKey>(.|\n)*)\[Private](?<PrivateKey>(.|\n)*)\[Master](?<MasterKey>(.|\n)*)\[Data]");
            Match result = extractInfo.Match(header);
            KeyChain keyChain = new KeyChain();
            keyChain.masterKeyID = result.Groups["KeyID"].Value;
            keyChain.publicKey = result.Groups["PublicKey"].Value;
            keyChain.privateKey = result.Groups["PrivateKey"].Value;
            keyChain.masterKey = result.Groups["MasterKey"].Value;
            return keyChain;
        }

        /// <summary>
        /// Masterkey weitergeben
        /// </summary>
        /// <param name="userKeyChain">Entschlüsselter Schlüsselbund des Benutzers</param>
        /// <param name="userIDToRelay">Benutzer (ID) dem der Key übergeben werden soll</param>
        /// <param name="KeyExchange">SQL-Zugriff zum Schlüsselaustausch</param>
        /// <param name="SelectPublicKeyOfUser">SQL-Select um Public-Key des Benutzers zu laden</param>
        public static void relayMasterKey(KeyChain userKeyChain, SqlDataSource KeyExchange, SqlDataSource SelectPublicKeyOfUser)
        { 
            //Testen ob bereits ein Schlüssel an den angegebenen Benutzer weitergegeben wurde
            DataView isAvalaibleRecord = (DataView)KeyExchange.Select(DataSourceSelectArguments.Empty);
            
            //Select gibt mindestens einen Record zurück => Schlüssel existiert bereits
            if (isAvalaibleRecord.Count < 1)
            {
                //PublicKey des angegebenen Benutzers laden
                DataView publicKeyRecord = (DataView)SelectPublicKeyOfUser.Select(DataSourceSelectArguments.Empty);

                //Wenn PublicKey geladen werden konnte
                if (publicKeyRecord.Count == 1)
                {
                    //PublicKey laden
                    String publicKey = publicKeyRecord[0]["PublicKey"].ToString();
                    //Masterkey mit PublicKey des gewählten Benutzers verschlüsseln
                    String masterKey = encryptKeyAsym(userKeyChain.masterKey, publicKey);
                    //Masterkey dem Benutzer zugeordnet in der DB speichern
                    KeyExchange.InsertParameters["MasterKey"].DefaultValue = masterKey;
                    KeyExchange.Insert();

                    //Defaultwerte des Inserts zurücksetzen
                    KeyExchange.InsertParameters["MasterKey"].DefaultValue = null;
                }
                else {
                    throw new Exception("Der Public-Key des gewählten Benutzers konnte nicht geladen werden.");
                }

            }
            else { 
                //Es ist zusätzlich ein Unique-Key in der Datenbank gesetzt um ein doppeltes Eintragen der Konstellation UserID - OwnerID zu verhindern
                throw new Exception("Ihr Schlüssel wurde bereits an den gewählten Benutzer weitergegeben.");
            }
        }

        /// <summary>
        /// Entschlüsselt eine Datei über den Header
        /// </summary>
        /// <param name="filename">Dateiname der Datei, die entschlüsselt werden soll</param>
        /// <param name="password">Passwort zum entschlüsseln</param>
        /// <returns>Pfad zur entschlüsselten Datei</returns>
        public static String decryptFileViaHeader(String filename, String password){
            //Initialisierungsvektor laden
            byte[] tDesIV = iv;

            //Filestreams für Input- und Output-File erzeugen.
            FileStream fin1 = new FileStream(SIT_Ressources.EncryptedPath + filename, FileMode.Open, FileAccess.Read);

            //Header laden
            LinkedList<String> headerList = new LinkedList<String>();
            byte[] headerByte = new byte[1000];
            int length = 0;

            //Headerlänge ermitteln
            bool DataTagFound = false;
            while (!DataTagFound && length < fin1.Length)
            {
                //Zeichen lesen
                length += fin1.Read(headerByte, 0, 1000);
                //Zeichen zwischenspeichern
                headerList.AddLast(byteArrayToString(headerByte));
                //Aktuellen String prüfen, ob der Datenblock erreicht wurde
                if (headerList.Last.Value.Contains("[Data]"))
                {
                    DataTagFound = true;
                }
                else
                {
                    //Position um 500 Zeichen nach vorne verlegen um String richtig zu lesen
                    fin1.Position -= 500;
                }
            }
            //Ersten Filestream schließen
            fin1.Close();

            //Wenn der Header richtig gesetzt wurde
            if (DataTagFound)
            {
                //Header erstellen
                StringBuilder headerBuilder = new StringBuilder();
                //Zähler um nur gerade Indizes zum Stringbuilder hinzuzufügen
                int i = 0;
                foreach (String current in headerList)
                {
                    //Wenn gerader Index, füge zum Stringbuilder hinzu
                    if (i % 2 == 0)
                    {
                        headerBuilder.Append(current);
                    }
                    i++;
                }

                //Filestream für Input-File erzeugen frisch erzeugen
                Stream fin2 = new FileStream(SIT_Ressources.EncryptedPath + filename, FileMode.Open, FileAccess.Read);

                //Header als String laden
                String header = headerBuilder.ToString();
                //String auf nötiges reduzieren
                Regex headerRegex = new Regex("\\[KeyID](.|\n)*\\[Public](.|\n)*\\[Private](.|\n)*\\[Master](.|\n)*\\[Data]");
                //Header zu Regex matchen
                Match headerMatch = headerRegex.Match(header);
                header = headerMatch.Value;

                //Wenn kein Ergebnis vom Regulären Ausdruck, dann wurde die Syntax der Datei geändert -> Abbruch
                if (header != null)
                {
                    //Erzeuge Schlüsselbund aus Headerstring
                    KeyChain headerKeyChain = getKeyChainFromHeader(header);

                    //Masterkey aus Tabelle mit eigenem PrivateKey entschlüsseln
                    headerKeyChain = decryptKeyChain(headerKeyChain, password);
                    String masterkey = headerKeyChain.masterKey;

                    if (masterkey != null)
                    {
                        //Header als Byte-Array speichern
                        headerByte = stringToByteArray(header);

                        //Header im Stream weglesen (wird nicht entschlüsselt)
                        fin2.Position = headerByte.Length;

                        //Outputstream anlegen
                        FileStream fout = new FileStream(SIT_Ressources.DecryptedPath + filename, FileMode.OpenOrCreate, FileAccess.Write);
                        fout.SetLength(0);

                        //Helfervariablen für Lesen und Schreiben.
                        byte[] bin = new byte[10000];   //Puffer fürs Entschlüsseln.
                        long rdlen = 0;                 //Anzahl der geschriebenen Bytes
                        long totlen = fin2.Length - headerByte.Length;       //Länge der Datei ohne Header
                        int len;                        //Anzahl der Bytes die auf einmal geschrieben werden sollen.

                        TripleDES tDes = new TripleDESCryptoServiceProvider();
                        byte[] masterkeyBit = stringToByteArray(masterkey);
                        //Masterkey überschreiben
                        masterkey = "";
                        CryptoStream decStream = new CryptoStream(fout, tDes.CreateDecryptor(masterkeyBit, tDesIV), CryptoStreamMode.Write);

                        try
                        {
                            //Aus dem Input-File lesen, entschlüsseln und in Output-File schreiben
                            while (rdlen < totlen)
                            {
                                len = fin2.Read(bin, 0, 10000);
                                decStream.Write(bin, 0, len);
                                rdlen = rdlen + len;
                            }

                        }
                        catch
                        {
                            //Alles Schließen
                            decStream.Close();
                            fout.Close();
                            fin2.Close();
                            throw new Exception("Fehler beim Entschlüsseln: Falsches Passwort?");
                        }
                        //Alles Schließen
                        decStream.Close();
                        fout.Close();
                        fin2.Close();

                        //Pfad der entschlüsselten Datei zurückgeben
                        return SIT_Ressources.DecryptedPath + filename;

                    }
                    else {
                        //Offenen Stream schließen
                        fin2.Close();
                        throw new Exception("Die Datei kann nicht entschlüsselt werden: Falsches Passwort?");
                    }
                }
                else
                {
                    //Offenen Stream schließen
                    fin2.Close();
                    throw new Exception("Die Datei kann nicht entschlüsselt werden, da sie nicht im gültigen Format vorliegt: Header fehlerhaft.");
                }
            }
            else
            {
                //Header konnte nicht gelesen werden
                throw new Exception("Die Datei kann nicht entschlüsselt werden, da sie nicht im gültigen Format vorliegt: Data-Tag nicht gefunden.");
            }
        
        }
    }

}