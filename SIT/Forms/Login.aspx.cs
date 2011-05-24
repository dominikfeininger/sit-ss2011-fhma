using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

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
                    ErrorLabel.Text = "ID in der Session: " + Session["ID"];
                    ErrorLabel.Visible = true;
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
    }
}