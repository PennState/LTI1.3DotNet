<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OAuth2POC._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>

    <h2>OAuth 2</h2>
    <table>
        <tr>
            <td><strong>Platform Issuer Id (OAuth2 "iss") </strong></td>
            <td>
                <asp:Label ID="lblLaunchIssuer" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Tool Client Id / Audience (OAuth2 "aud"):</strong></td>
            <td>
                <asp:Label ID="lblLaunchAudience" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Platform Permanent User ID (OAuth2 "sub"):</strong></td>
            <td>
                <asp:Label ID="lblSubjectId" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Platform Public Key Set Discovery URL:</strong></td>
            <td>
                <asp:Label ID="lblKeySetUrl" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Replay Prevention (OAuth2 "nonce"):</strong></td>
            <td>
                <asp:Label ID="lblNonce" runat="server"></asp:Label></td>
        </tr>
        </table>
    <h2>User</h2>
    <table>
        <tr>
            <td><strong>User Name:</strong></td>
            <td>
                <asp:Label ID="lblUserName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td><strong>User Email:</strong></td>
            <td>
                <asp:Label ID="lblEmail" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>User Roles:</strong></td>
            <td>
                <asp:BulletedList ID="blRoles" runat="server"></asp:BulletedList>
            </td>
        </tr>
        </table>
    <h2>Platform</h2>
    <table>
        <tr>
            <td><strong>Platform:</strong></td>
            <td>
                <asp:Label ID="lblPlatform" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Platform Context (e.g. Course/Group/Section):</strong></td>
            <td>
                <asp:Label ID="lblContextCourse" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Platform LTI Launch Link:</strong></td>
            <td>
                <asp:Label ID="lblLaunchLink" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Platform Launch Link Custom Claims:</strong></td>
            <td>
                <asp:BulletedList ID="blCustom" runat="server"></asp:BulletedList>
            </td>
        </tr>
        <tr>
            <td><strong>Platform LTI Presentation:</strong></td>
            <td>
                <asp:Label ID="lblLaunchPresentation" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td><strong>Scopes:</strong></td>
            <td>
                <asp:BulletedList ID="blScope" runat="server"></asp:BulletedList>
            </td>
        </tr>
    </table>

    
</asp:Content>
