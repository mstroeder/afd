<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="AFDApp.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label runat="server" Text="FileName"></asp:Label>
<asp:DropDownList ID="cmbFileName" runat="server" AutoPostBack="True">
<asp:ListItem Value="All">All</asp:ListItem>
</asp:DropDownList>
<asp:Label runat="server" Text="Show"></asp:Label>
<asp:DropDownList ID="cmbErrorsFilter" runat="server" AutoPostBack="True">
<asp:ListItem Value="All">All</asp:ListItem>
<asp:ListItem Value="errorsonly">Errors Only</asp:ListItem>
<asp:ListItem Value="successonly">Successful Only</asp:ListItem>
</asp:DropDownList>
    <asp:Button ID="GetDataButton" runat="server" Text="Get Data"  onclick="GetDataButton_Click"/>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="id" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="30">
        <Columns>
            <asp:BoundField DataField="id" HeaderText="id" ReadOnly="True" SortExpression="id" />
            <asp:BoundField DataField="property" HeaderText="property" SortExpression="property" />
            <asp:BoundField DataField="customer" HeaderText="customer" SortExpression="customer" />
            <asp:BoundField DataField="action" HeaderText="action" SortExpression="action" />
            <asp:BoundField DataField="value" HeaderText="value" SortExpression="value" />
            <asp:BoundField DataField="file" HeaderText="file" SortExpression="file" />
            <asp:BoundField DataField="uploadstatus" HeaderText="uploadstatus" SortExpression="uploadstatus" />
            <asp:BoundField DataField="hash" HeaderText="hash" SortExpression="hash" />
            <asp:BoundField DataField="uploaderror" HeaderText="uploaderror" SortExpression="uploaderror" />
            <asp:BoundField DataField="checkResult" HeaderText="checkResult" SortExpression="checkResult" />
            <asp:BoundField DataField="checkerror" HeaderText="checkerror" SortExpression="checkerror" />
        </Columns>
        <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NextPreviousFirstLast" NextPageText="Next" Position="TopAndBottom" PreviousPageText="Prev" />
    </asp:GridView>
</asp:Content>
