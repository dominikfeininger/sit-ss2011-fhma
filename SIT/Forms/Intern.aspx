<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Intern.aspx.cs" Inherits="SIT.Forms.Intern" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style2
        {
            width: 260px;
            font-size: large;
        }
        .style3
        {
            width: 245px;
            text-align: right;
        }
        .style4
        {
            width: 360px;
        }
    </style>
</head>
<body alink="#ffffff" bgcolor="#00496c" text="#ffffff" vlink="#66ccff" 
    link="#9dd1f4">
    <form id="form1" runat="server">
    <div style="margin: 0px; padding: 50px; font-family: Arial, Helvetica, Sans-Serif;" 
        align="center">
    <asp:Panel ID="MainFrame" runat="server" Height="16px" Visible="False">
        <div style="border-style: solid; border-width: thin thin thick thin; border-color: #2380B1 #2380B1 #005B88 #2380B1; padding: 5px; background-color: #006FA4; text-align: left;">
            <table style="width:100%;">
                <tr>
                    <td class="style2">
                        <strong>Interner Bereich</strong></td>
                    <td style="text-align: right">
                        <asp:Button ID="logout" runat="server" onclick="logout_Click" 
                            style="text-align: right" Text="Logout" />
                    </td>
                </tr>
            </table>
        </div>
        <div style="padding: 10px; border: thin solid #2380B1; background-color: #005B88; margin-top: 10px; z-index: auto;">
            Private Key Secret:
            <asp:TextBox ID="privateSecret" runat="server" TextMode="Password"></asp:TextBox>
        </div>
        <div style="padding-top: 10px; padding-bottom: 10px;">
            <asp:Label ID="Error" runat="server" BackColor="#00496C" BorderColor="White" 
                BorderStyle="Dotted" BorderWidth="1px" Text="Fehler" Visible="False"></asp:Label>
        </div>
        <div style="padding: 10px 0px 0px 0px; border: thin solid #2380B1; background-color: #005B88; margin-top: 0px; z-index: auto; text-align: left;">
            &nbsp;&nbsp; <strong>Datei verschlüsseln und entschlüsseln</strong>
            <div style="border-color: #2380B1; margin-top: 10px; padding: 10px; background-color: #106898; border-top-style: solid;">
                <table style="width:100%;">
                    <tr>
                        <td class="style4">
                            Datei:</td>
                        <td class="style3">
                        </td>
                    </tr>
                    <tr>
                        <td class="style4">
                            <asp:FileUpload ID="UploadFile" runat="server" />
                        </td>
                        <td class="style3">
                            <asp:Button ID="uploadButton" runat="server" onclick="uploadButton_Click" 
                                Text="Verschlüsseln" />
                            <asp:Button ID="decryptButton" runat="server" onclick="decryptButton_Click" 
                                style="text-align: center" Text="Entschlüsseln" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div style="padding: 10px 0px 0px 0px; border: thin solid #2380B1; background-color: #005B88; margin-top: 20px; z-index: auto; text-align: left;">
            &nbsp;&nbsp; <strong>Schlüsselverwaltung</strong>
            <div style="border-color: #2380B1; margin-top: 10px; padding: 10px; background-color: #106898; border-top-style: solid;">
                <div style="border: thin dashed #5BB1DF; padding: 5px; background-color: #0077B0;">
                    Erhaltene Schlüssel<div 
                        style="border: thin solid #0D557B; margin: 7px 0px 0px 0px; padding: 5px; background-color: #116C9D;">
                        Sie besitzen die Schlüssel der folgenden Benutzer:<br />
                        <br />
                        <asp:GridView ID="AllKeysView" runat="server" AllowPaging="True" 
                            AutoGenerateColumns="False" CellPadding="2" CellSpacing="1" 
                            DataSourceID="SelectAllKeys">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="Benutzername" 
                                    SortExpression="Name" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div style="border: thin dashed #5BB1DF; padding: 5px; background-color: #0077B0; margin-top: 10px;">
                    Weitergereichte Schlüssel<div 
                        style="border: thin solid #0D557B; margin: 7px 0px 0px 0px; padding: 5px; background-color: #116C9D;">
                        <asp:GridView ID="RelayedKeys" runat="server" AllowPaging="True" 
                            AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="SchlüsselID" 
                            DataSourceID="EditKeys" onrowdeleted="RelayedKeys_RowDeleted" 
                            onrowdeleting="RelayedKeys_RowDeleting">
                            <Columns>
                                <asp:BoundField DataField="SchlüsselID" HeaderText="SchlüsselID" 
                                    InsertVisible="False" ReadOnly="True" SortExpression="SchlüsselID" />
                                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                                <asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Löschen" />
                            </Columns>
                        </asp:GridView>
                        </div>
                </div>
                <div style="border: thin dashed #5BB1DF; padding: 5px; background-color: #0077B0; margin-top: 10px;">
                    Schlüssel weiterreichen
                    <div 
                        style="border: thin solid #0D557B; margin: 7px 0px 0px 0px; padding: 5px; background-color: #116C9D;">
                        <asp:DropDownList ID="UserSelect" runat="server" DataSourceID="SelectAllUsers" 
                            DataTextField="Name" DataValueField="ID">
                        </asp:DropDownList>
                        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
                            Text="Weiterreichen" />
                    </div>
                </div>
            </div>
        </div>
        <asp:SqlDataSource ID="GetMasterKey" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT [OwnerID], [MasterKey] FROM [MasterKeys] WHERE (([UserID] = @UserID) AND ([OwnerID] = @OwnerID))">
            <SelectParameters>
                <asp:SessionParameter Name="UserID" SessionField="ID" Type="Int32" />
                <asp:Parameter Name="OwnerID" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SelectPublicKey" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT Users.PublicKey, Users.PrivateKey, MasterKeys.MasterKey, MasterKeys.ID FROM Users INNER JOIN MasterKeys ON Users.ID = MasterKeys.UserID WHERE (Users.ID = @ID)">
            <SelectParameters>
                <asp:SessionParameter Name="ID" SessionField="ID" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="checkPrivatePasswort" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT [ID], [PrivateKeyPassword] FROM [Users] WHERE (([ID] = @ID) AND ([PrivateKeyPassword] = @PrivateKeyPassword))">
            <SelectParameters>
                <asp:SessionParameter Name="ID" SessionField="ID" Type="Int32" />
                <asp:Parameter Name="PrivateKeyPassword" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SelectAllKeys" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT Users.name AS Name FROM MasterKeys INNER JOIN Users ON MasterKeys.OwnerID = Users.ID WHERE (MasterKeys.UserID = @user )">
            <SelectParameters>
                <asp:SessionParameter Name="user" SessionField="ID" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="EditKeys" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            DeleteCommand="DELETE FROM MasterKeys WHERE (ID = @ID)" 
            SelectCommand="SELECT MasterKeys.ID AS SchlüsselID, Users.name AS Name FROM MasterKeys INNER JOIN Users ON MasterKeys.UserID = Users.ID WHERE (MasterKeys.OwnerID = @ID) AND (MasterKeys.IsCopy = 1)">
            <DeleteParameters>
                <asp:Parameter Name="ID" />
            </DeleteParameters>
            <SelectParameters>
                <asp:SessionParameter Name="ID" SessionField="ID" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="KeyExchange" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            InsertCommand="INSERT INTO MasterKeys(UserID, MasterKey, IsCopy, OwnerID) VALUES (@UserID, @MasterKey, 1, @OwnerID)" 
            SelectCommand="SELECT ID, MasterKey, IsCopy, OwnerID, UserID FROM MasterKeys WHERE (UserID = @UserID) AND (OwnerID = @OwnerID)">
            <InsertParameters>
                <asp:Parameter Name="UserID" />
                <asp:Parameter Name="MasterKey" />
                <asp:SessionParameter Name="OwnerID" SessionField="ID" />
            </InsertParameters>
            <SelectParameters>
                <asp:Parameter Name="UserID" />
                <asp:SessionParameter Name="OwnerID" SessionField="ID" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SelectAllUsers" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT [name] as Name, [ID] FROM [Users]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SelectPublicKeyOfUser" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT [ID], [PublicKey] FROM [Users] WHERE ([ID] = @ID)">
            <SelectParameters>
                <asp:Parameter Name="ID" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
    </asp:Panel>
    </div>
    
    <br />
    </form>
</body>
</html>
