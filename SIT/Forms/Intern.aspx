<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Intern.aspx.cs" Inherits="SIT.Forms.Intern" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:Panel ID="MainFrame" runat="server" Height="673px" Visible="False">
        <asp:Button ID="logout" runat="server" onclick="logout_Click" Text="Logout" />
        <br />
        <br />
        <br />
        Datei auswählen:<br />
        <asp:FileUpload ID="UploadFile" runat="server" />
        <br />
        <br />
        <asp:Button ID="uploadButton" runat="server" onclick="uploadButton_Click" 
            Text="Verschlüsseln" />
        &nbsp;<asp:Button ID="decryptButton" runat="server" 
            onclick="decryptButton_Click" Text="Entschlüsseln" />
        <br />
        <asp:Label ID="Error" runat="server" Text="Fehler" Visible="False"></asp:Label>
        <br />
        <br />
        Private Key Secret:<br /> &nbsp;<asp:TextBox ID="privateSecret" runat="server" 
            TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <br />
        <asp:Button ID="KeyExchangeButton" runat="server" onclick="change_Click" 
            Text="Schlüsselverwaltung" />
        <br />
        <asp:Panel ID="KeyExchangePanel" runat="server" Visible="False">
            <br />
            Sie besitzen die Schlüssel von folgenden Benutzern:<br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                DataSourceID="SelectAllKeys">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SelectAllKeys" runat="server" 
                ConnectionString="<%$ ConnectionStrings:SIT_Database %>" SelectCommand="SELECT Users.name as Name FROM MasterKeys INNER JOIN Users ON MasterKeys.OwnerID = Users.ID
WHERE Users.ID = @user">
                <SelectParameters>
                    <asp:SessionParameter Name="user" SessionField="ID" />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            Schlüssel Weitergeben:<br />
            <asp:DropDownList ID="UserSelect" runat="server" DataSourceID="SelectAllUsers" 
                DataTextField="Name" DataValueField="ID" 
                onselectedindexchanged="UserSelect_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            <br />
            <asp:SqlDataSource ID="SelectAllUsers" runat="server" 
                ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
                SelectCommand="SELECT [name] as Name, [ID] FROM [Users]">
            </asp:SqlDataSource>
            <br />
            <br />
        </asp:Panel>
    </asp:Panel>
    <br />
    <asp:SqlDataSource ID="checkPrivatePasswort" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
        SelectCommand="SELECT [ID], [PrivateKeyPassword] FROM [Users] WHERE (([ID] = @ID) AND ([PrivateKeyPassword] = @PrivateKeyPassword))">
        <SelectParameters>
            <asp:SessionParameter Name="ID" SessionField="ID" Type="Int32" />
            <asp:Parameter Name="PrivateKeyPassword" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
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
    </form>
</body>
</html>
