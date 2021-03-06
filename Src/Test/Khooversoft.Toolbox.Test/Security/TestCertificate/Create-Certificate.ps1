<# 
.NOTES 
    Name: Create-Certificate.ps1
    Author: Kelvin Hoover

.SYNOPSIS 
    Execute PowerShell self signed certificate to create certificate

.DESCRIPTION 
    This script will export a database model against a target using the SqlPackage.exe utility.

.PARAMETER Source 
    DACPACK source file to use.

.PARAMETER TraceFile
    File to output trace information

#>

#Requires -RunAsAdministrator
 
Param (
    [ValidateSet(“Vault”, ”Client”, "JwtTest")]
    [string] $Type,

    [switch] $Force
)

$ErrorActionPreference = "Stop";

$certTypes = @{
    "Vault" = @{
        "File" = "VaultData.khoover.com.pfx"
        "DnsNames" = @("VaultData.khoover.com")
        "CertLocation" = "cert:\LocalMachine\My"
        "Password" = "password"
    }
    "Client" = @{
        "File" = "VaultClient.khoover.com.pfx"
        "DnsNames" = @("VaultClient.khoover.com")
        "CertLocation" = "cert:\CurrentUser\my"
        "Password" = "password"
    }
    "JwtTest" = @{
        "File" = "JwtVaultTest.khoover.com.pfx"
        "DnsNames" = @("JwtVaultTest.khoover.com")
        "CertLocation" = "cert:\LocalMachine\My"
        "Password" = "password"
    }
}

Write-Host "Building certificate for $Type";

$selection = $certTypes[$Type];

if( -not $Force )
{
    if( Test-Path $selection.File )
    {
        throw "Cannot continue, file $($selection.File) already exists.  Delete it or use the -Force option";
    }
}

$password = $selection.Password

if( !$password )
{
    $password = Read-Host -Prompt "Enter password for PFX" -AsSecureString;
}
else
{
    $password = ConvertTo-SecureString -String $password -AsPlainText -Force;
}

Remove-Item $selection.File -ErrorAction SilentlyContinue

try
{
    $cert = New-SelfSignedCertificate -DnsName $selection.DnsNames -CertStoreLocation $selection.CertLocation -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider";

    $certPath = "$($selection.CertLocation)\$($cert.Thumbprint)";

    Export-PfxCertificate -Password $password -FilePath $selection.File -Cert $certPath;
}
finally
{
    if( $certPath )
    {
        Remove-Item $certPath;
    }
}
