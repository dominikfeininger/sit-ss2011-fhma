using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace SIT
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Anmelden_Click(object sender, EventArgs e)
        {
            //Fehlerpanel zurücksetzen
            ErrorLabel.Visible = false;
            //Wenn Werte eingegeben wurden
            if(!UserTextbox.Text.Equals("") && !PasswordTextbox.Text.Equals("")){
                //Benutzereingabe laden
                String user = UserTextbox.Text;
                String password = PasswordTextbox.Text;
                //Setze das Passwort für die Abfrage
                DataSourceCheckPW.SelectParameters["Password"].DefaultValue = SecurityProvider.hashPassword(PasswordTextbox.Text);
                //Benutze das angelegte Control zum Select
                DataView dv = (DataView)DataSourceCheckPW.Select(DataSourceSelectArguments.Empty);
                if (dv.Count == 1)
                {
                    //ID des Benutzers in der Session hinterlegen
                    Session.Add("ID", dv[0]["ID"]);

                    //Zum Internen Bereich weiterleiten
                    HttpContext.Current.Response.Redirect("~/Forms/Intern.aspx");


                }
                else {
                    //Fehlermeldung anzeigen
                    ErrorLabel.Text = "Sie haben einen falschen Benutzernamen oder ein flasches Passwort eingeben.";
                    ErrorLabel.Visible = true;      
                }
            }
            else
            {
                //Fehlermeldung anzeigen
                ErrorLabel.Text = "Sie müssen beide Felder ausfüllen.";
                ErrorLabel.Visible = true;
            }
        }

        /// <summary>
        /// Datei ohne Datenbankanbindung entschlüsseln
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DecryptWithHeaderButton_Click(object sender, EventArgs e)
        {
            ErrorLabel.Visible = false;
            //Quelle prüfen
            if (UploadFile.HasFile)
            {
                //Wenn kein Passwort eingegeben
                if (!SuperPW.Text.Equals(""))
                {
                    //Fileinfo zur abgelegten Datei holen, um später wieder löschen zu können
                    FileInfo savedFile = new FileInfo(SIT_Ressources.EncryptedPath + UploadFile.FileName);

                        try
                        {
                            //Datei aus dem Fileupload speichern
                            UploadFile.SaveAs(SIT_Ressources.EncryptedPath + UploadFile.FileName);
                            sendFile(SecurityProvider.decryptFileViaHeader(UploadFile.FileName, SuperPW.Text));
                            //gespeicherte Datei löschen
                            savedFile.Delete();
                        }
                        catch (Exception ex)
                        {
                            //gespeicherte Datei löschen
                            savedFile.Delete();
                            ErrorLabel.Text = "Fehler beim Entschlüsseln der Datei " + UploadFile.PostedFile.FileName + ": " + ex.Message;
                            ErrorLabel.Visible = true;
                        }
                }
                else
                {
                    ErrorLabel.Text = "Das Secret für den Privatekey muss gewählt werden";
                    ErrorLabel.Visible = true;
                }
            }
            else
            {
                ErrorLabel.Text = "Eine Datei muss gewählt werden";
                ErrorLabel.Visible = true;
            }
        }

        /// <summary>
        /// Sendet dem Benutzer eine Datei
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
            HttpContext.Current.Response.TransmitFile(downloadFile.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Close();
            downloadFile.Delete();
            //Response.End() darf nicht gerufen werden (http://support.microsoft.com/kb/312629/EN-US/) löst eine Exception aus
            //HttpContext.Current.Response.End();

        }
    }
}