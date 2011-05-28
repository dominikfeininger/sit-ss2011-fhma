using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace SIT.Forms
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void registerButton_Click(object sender, EventArgs e)
        {
            ErrorLabel.Visible = false;
            //Validierung ob alle Felder ausgefüllt sind
            if (!(UserTextbox.Text.Equals("") ||
                PasswordTextbox1.Text.Equals("") ||
                PasswordTextbox2.Text.Equals("") ||
                PrivateKeyPassword1.Text.Equals("") ||
                PrivateKeyPassword2.Text.Equals("")))
            {
                //Passwörter stimmen nicht überein
                if (!PasswordTextbox1.Text.Equals(PasswordTextbox2.Text)) 
                {
                    ErrorLabel.Text = "Die Passwörter müssen übereinstimmen";
                    ErrorLabel.Visible = true;                    
                }
                //Passwörter für PublicKey stimmen nicht überein
                else if (!PrivateKeyPassword1.Text.Equals(PrivateKeyPassword2.Text))
                {
                    ErrorLabel.Text = "Die Passwörter für den Private-Key stimmen nicht überein";
                    ErrorLabel.Visible = true;
                }
                //Alle Angaben korrekt - speichere in Datenbank
                else 
                {
                    try
                    {
                        //Schlüsselbund generieren
                        KeyChain keys = SecurityProvider.createKeys(PrivateKeyPassword1.Text);
                        SqlAddNewUser.InsertParameters["name"].DefaultValue = UserTextbox.Text;
                        SqlAddNewUser.InsertParameters["password"].DefaultValue = SecurityProvider.hashPassword(PasswordTextbox1.Text);
                        SqlAddNewUser.InsertParameters["PublicKey"].DefaultValue = keys.publicKey;
                        SqlAddNewUser.InsertParameters["PrivateKey"].DefaultValue = keys.privateKey;
                        SqlAddNewUser.InsertParameters["PrivateKeyPassword"].DefaultValue = SecurityProvider.hashPassword(PrivateKeyPassword1.Text);
                        SqlAddNewUser.Insert();

                        //Keine Exception bis hier? - Dann ist die Anmeldung erolgreich in der Datenbank gespeichert
                        //SELECT um die ID des Benutzers zu laden
                        SqlAddNewUser.SelectParameters["name"].DefaultValue = UserTextbox.Text;
                        DataView record = (DataView)SqlAddNewUser.Select(DataSourceSelectArguments.Empty);
                        String ID = record[0]["ID"].ToString();

                        try { 
                            //Insert anpassen
                            SqlAddNewMasterkey.InsertParameters["UserID"].DefaultValue = ID;
                            SqlAddNewMasterkey.InsertParameters["Masterkey"].DefaultValue = keys.masterKey;
                            SqlAddNewMasterkey.InsertParameters["OwnerID"].DefaultValue = ID;
                            SqlAddNewMasterkey.Insert();

                            //Zum login weiterleiten
                            HttpContext.Current.Response.Redirect("~/Forms/Login.aspx");
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            //Wird ausgelöst, wenn ein Einfügen nicht möglich war
                            ErrorLabel.Text = "Es ist ein Fehler bei der Auswahl des Benutzers aufgetreten.";
                            ErrorLabel.Visible = true;

                        }

                    }
                    catch (System.Data.SqlClient.SqlException){ 
                        //Wird ausgelöst, wenn Benutzername vergeben
                        ErrorLabel.Text = "Benutzername ist bereits vorhanden, bitte wählen Sie einen anderen Benutzernamen.";
                        ErrorLabel.Visible = true;

                    }
                }
            }
            else {
                ErrorLabel.Text = "Es müssen alle Felder ausgefüllt sein";
                ErrorLabel.Visible = true;
            }
        }
    }
}