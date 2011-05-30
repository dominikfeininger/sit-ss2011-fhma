using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Data;

namespace SIT.Forms
{
    public partial class Intern : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //TODO auskommentieren, damit man sich einloggen muss
            if (Session["ID"] != null)
            {
                MainFrame.Visible = true;
            }
            else {
                //Einloggen
                HttpContext.Current.Response.Redirect("~/Forms/Login.aspx");
            }
            //Session.Add("ID", "7");
        }

        /// <summary>
        /// Button um sich auszuloggen (Die Session zu löschen)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void logout_Click(object sender, EventArgs e)
        {
            //Session löschen
            Session.Clear();
            //Weiterleitung zum Login
            HttpContext.Current.Response.Redirect("~/Forms/Login.aspx");
        }

        /// <summary>
        /// Button um Datei zu verschlüsseln
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void uploadButton_Click(object sender, EventArgs e)
        {
            Error.Visible = false;
            //Quelle prüfen
            if (UploadFile.HasFile)
            {
                //Wenn kein Passwort eingegeben
                if (!privateSecret.Text.Equals(""))
                {

                    //Passwort überprüfen
                    checkPrivatePasswort.SelectParameters["PrivateKeyPassword"].DefaultValue = SecurityProvider.hashPassword(privateSecret.Text);
                    //Benutze das angelegte Control zum Select
                    DataView dv1 = (DataView)checkPrivatePasswort.Select(DataSourceSelectArguments.Empty);
                    //Wenn es genau einen Record gibt wurde das richtige Passwort gewählt
                    if (dv1.Count == 1)
                    {
                        try
                        {
                            //Schlüsselbund erzeugen und entschlüsseln
                            KeyChain encryptedKeychain = getCurrentKeyChain();
                            //Datei verschlüsseln und zurüxksenden
                            sendFile(SecurityProvider.encryptFile(UploadFile.PostedFile.InputStream, UploadFile.FileName, encryptedKeychain));
                            //Meldung anzeigen
                            Error.Text = "Die Datei '" + UploadFile.PostedFile.FileName + "' wurde erfolgreich verschlüsselt.";
                            Error.Visible = true;

                        }
                        catch (Exception ex)
                        {
                            Error.Text = "Fehler beim Speichern von " + UploadFile.PostedFile.FileName + ": " + ex.Message;
                            Error.Visible = true;
                        }
                    }
                    else
                    {
                        Error.Text = "Das Secret für den Privatekey wurde falsch eingegeben";
                        Error.Visible = true;
                    }
                }
                else
                {
                    Error.Text = "Das Secret für den Privatekey muss gewählt werden";
                    Error.Visible = true;
                }
            }
            else
            {
                Error.Text = "Eine Datei muss gewählt werden";
                Error.Visible = true;
            }
        }

        /// <summary>
        /// Button um Datei zu entschlüsseln
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void decryptButton_Click(object sender, EventArgs e)
        {
            Error.Visible = false;
            //Quelle prüfen
            if (UploadFile.HasFile)
            {
                //Wenn kein Passwort eingegeben
                if (!privateSecret.Text.Equals(""))
                {

                    //Passwort überprüfen
                    checkPrivatePasswort.SelectParameters["PrivateKeyPassword"].DefaultValue = SecurityProvider.hashPassword(privateSecret.Text);
                    //Benutze das angelegte Control zum Select
                    DataView dv1 = (DataView)checkPrivatePasswort.Select(DataSourceSelectArguments.Empty);
                    //Wenn es genau einen Record gibt wurde das richtige Passwort gewählt
                    if (dv1.Count == 1)
                    {
                        try
                        {
                            //Datei aus dem Fileupload speichern
                            UploadFile.SaveAs(SIT_Ressources.EncryptedPath + UploadFile.FileName);
                            //Schlüsselbund erzeugen und entschlüsseln
                            KeyChain encryptedKeychain = getCurrentKeyChain();
                            sendFile(SecurityProvider.decryptFile(UploadFile.FileName, GetMasterKey, encryptedKeychain));
                        }
                        catch (Exception ex)
                        {
                            Error.Text = "Fehler beim Entschlüsseln der Datei " + UploadFile.PostedFile.FileName + ": " + ex.Message;
                            Error.Visible = true;
                        }
                    }
                    else
                    {
                        Error.Text = "Das Secret für den Privatekey wurde falsch eingegeben";
                        Error.Visible = true;
                    }
                }
                else
                {
                    Error.Text = "Das Secret für den Privatekey muss gewählt werden";
                    Error.Visible = true;
                }
            }
            else
            {
                Error.Text = "Eine Datei muss gewählt werden";
                Error.Visible = true;
            }
        }

        /// <summary>
        /// Sendet dem Benutzer eine Daten
        /// </summary>
        /// <param name="filename">Dateiname der Datei, die gesendet wurde</param>
        /// <param name="filepath">Pfad zur Datei im System</param>
        private void sendFile(String filepath)
        {
            FileInfo downloadFile = new FileInfo(filepath);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", downloadFile.Name));
            HttpContext.Current.Response.AddHeader("Content-Length", downloadFile.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.WriteFile(downloadFile.FullName);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// Lädt einen Schlüsselbund zum aktullen Benutzer aus der Datenbank und entschlüsselt diesen
        /// </summary>
        /// <returns>Entschlüsselter Schlüsselbund des aktuellen Benutzers</returns>
        private KeyChain getCurrentKeyChain()
        {
            //Select absetzen
            DataView dv = (DataView)SelectPublicKey.Select(DataSourceSelectArguments.Empty);
            //KeyChain erstellen
            KeyChain keyChain = new KeyChain();
            //Key in den Schlüsselbund laden
            keyChain.publicKey = dv[0]["PublicKey"].ToString();
            keyChain.privateKey = dv[0]["PrivateKey"].ToString();
            keyChain.masterKey = dv[0]["MasterKey"].ToString();
            keyChain.masterKeyID = Session["ID"].ToString();
            //Schlüsselbund entschlüsseln
            KeyChain encryptedKeychain = SecurityProvider.decryptKeyChain(keyChain, privateSecret.Text);
            return encryptedKeychain;
        }

        /// <summary>
        /// Schlüsselweitergabe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            //Schlüsselbund holen
            Error.Visible = false;
            //Wenn kein Passwort eingegeben
            if (!privateSecret.Text.Equals(""))
            {
                //Wenn ein Benutzer geählt wurde
                if (UserSelect.SelectedItem.Value != null) {
                    //Passwort überprüfen
                    checkPrivatePasswort.SelectParameters["PrivateKeyPassword"].DefaultValue = SecurityProvider.hashPassword(privateSecret.Text);
                    //Benutze das angelegte Control zum Select
                    DataView dv1 = (DataView)checkPrivatePasswort.Select(DataSourceSelectArguments.Empty);
                    //Wenn es genau einen Record gibt wurde das richtige Passwort gewählt
                    if (dv1.Count == 1)
                    {
                        try
                        {
                            //Schlüsselbund, BenutzerID und Datenverbindung übergeben
                            SecurityProvider.relayMasterKey(getCurrentKeyChain(), UserSelect.SelectedItem.Value, KeyExchange, SelectPublicKeyOfUser);
                            //Wenn keine Exception bis hier, dann wurde dem Benutzer der Schlüssel übergeben
                            Error.Text = "Der Schlüssel wurde erfolgreich an den Benutzer '" + UserSelect.SelectedItem.ToString() +"' weitergegeben.";
                            Error.Visible = true;
                            //Seite neu laden
                            HttpContext.Current.Response.Redirect("~/Forms/Intern.aspx");
                        }
                        catch (Exception ex)
                        {
                            Error.Text = "Fehler beim Übertragen des Schlüssels: " + ex.Message;
                            Error.Visible = true;
                        }
                    }
                    else
                    {
                        Error.Text = "Das Secret für den Privatekey wurde falsch eingegeben";
                        Error.Visible = true;
                    }
                
                } 
            }
            else
            {
                Error.Text = "Das Secret für den Privatekey muss gewählt werden";
                Error.Visible = true;
            }
        }

        //Schlüssel eingezogen
        protected void RelayedKeys_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            //Neu laden
            HttpContext.Current.Response.Redirect("~/Forms/Intern.aspx");
        }

        //Schlüssel einziehen
        protected void RelayedKeys_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //Tabellenzelle der ID holen und zum Deletestatement hinzufügen
            EditKeys.DeleteParameters["ID"].DefaultValue = RelayedKeys.Rows[e.RowIndex].Cells[0].Text;
            EditKeys.Delete();
            //Default zurücksetzen
            EditKeys.DeleteParameters["ID"].DefaultValue =String.Empty;
        }
    }
}