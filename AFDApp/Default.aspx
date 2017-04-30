<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AFDApp._Default" Async="true" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="FileLabel" runat="server" Text="Select file to upload and process:"></asp:Label>
    <asp:FileUpload ID="FileUploadControl1" runat="server" />
    <asp:Button ID="UploadButton" runat="server" Text="Upload File"  onclick="UploadButton_Click"/>
    <br /><br />
    <asp:Label ID="StatusLabel" runat="server" Text="Upload Status:"></asp:Label>
</asp:Content>
