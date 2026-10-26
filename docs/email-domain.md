# Email on the domain

Date: 2026-10-26

How `laztty.com` receives and sends email: Cloudflare Email Routing forwards incoming mail to personal inboxes, and Resend sends mail, both what the API sends and replies written from Gmail. Replaces the pilot's Gmail account as the API's sender ([pilot deployment](pilot-deployment.md#email)).

## Decision

| Piece | Choice | Why |
|---|---|---|
| Receiving | Cloudflare Email Routing | Free, the domain is already on Cloudflare DNS ([Domain and DNS](domain-and-dns.md)), no mailbox to pay for: each address forwards to an existing inbox |
| Sending from the API | Resend over SMTP | The API already sends through SMTP (`SmtpEmailTransport`), so only configuration changes. Mail leaves from `@laztty.com` signed with DKIM, which lands in the inbox more often than a Gmail address |
| Replying as `@laztty.com` | Gmail **Send mail as**, through Resend's SMTP | Email Routing only receives; Gmail needs an SMTP server to send as another address |
| Mailboxes (Google Workspace, Zoho, Microsoft 365) | Not now | Worth it when the team needs shared inboxes or more than a couple of addresses. Moving to one replaces the Email Routing `MX` records; the Resend records stay |

### Addresses

| Address | Use | Forwards to |
|---|---|---|
| `hola@laztty.com` | Contact for people (businesses, students) | The owner's inbox |
| `no-reply@laztty.com` | `From` of every email the API sends | The owner's inbox, so replies and bounces sent by people anyway are not lost |

### Limits

Resend's free plan allows 3,000 emails per month and 100 per day, for one domain. That is fewer per day than Gmail (about 500), and the replies sent from Gmail through Resend count too. It is enough for the pilot's password resets, sign-up confirmations and feedback; before turning on [automatic reminders](backend/20261021-automatic-reminders/plan.md) by email, move to a paid plan or check the daily volume. Check the current limits at <https://resend.com/pricing>.

## Records

Every record is **DNS only**; `MX` and `TXT` records can't be proxied anyway ([why DNS only](domain-and-dns.md#why-dns-only)). Copy the exact values from each dashboard: the ones below show their shape.

| Type | Name | Value | Added by |
|---|---|---|---|
| `MX` | `@` | `route1.mx.cloudflare.net`, `route2.mx.cloudflare.net`, `route3.mx.cloudflare.net` (priorities set by Cloudflare) | Email Routing |
| `TXT` | `@` | `v=spf1 include:_spf.mx.cloudflare.net ~all` | Email Routing |
| `MX` | `send` | `feedback-smtp.eu-west-1.amazonses.com`, priority `10` | Resend |
| `TXT` | `send` | `v=spf1 include:amazonses.com ~all` | Resend |
| `TXT` | `resend._domainkey` | `p=<public key shown by Resend>` | Resend |
| `TXT` | `_dmarc` | `v=DMARC1; p=none; rua=mailto:hola@laztty.com` | By hand |

The two SPF records don't clash: Cloudflare's is on the apex and Resend's on `send`, the bounce domain Resend uses. There is only one SPF record per name; if another service later needs the apex, add its `include:` to the existing record instead of creating a second one.

DMARC starts at `p=none`, which only reports. After a few weeks of reports showing every legitimate email passing (Resend and Gmail through Resend), raise it to `p=quarantine`.

## Setup

Run once, in this order: receiving first, because Gmail and Resend send verification codes to the new address.

### 1. Email Routing (receiving)

1. In Cloudflare, open `laztty.com` → **Email → Email Routing → Get started**.
2. **Destination addresses → Add**: the owner's personal address. Cloudflare sends a confirmation link; open it.
3. **Routing rules → Create address**: `hola` → that destination. Repeat for `no-reply`.
4. **Enable Email Routing**. Cloudflare offers to add the `MX` and SPF `TXT` records above; accept. It refuses if the apex already has `MX` records from another host.

Check it: an email sent from another account to `hola@laztty.com` arrives in the personal inbox.

### 2. Resend (sending)

1. Create an account at <https://resend.com> with the owner's address.
2. **Domains → Add domain**: `laztty.com`, region **Ireland (eu-west-1)**, the same region as the rest of the pilot's data (Azure France Central, Sentry EU).
3. Create the `MX`, SPF and DKIM records Resend shows in Cloudflare (**DNS → Records → Add record**), DNS only. Resend can also add them itself through Cloudflare's sign-in; either works. Add the `_dmarc` record by hand.
4. **Verify DNS records**. It takes minutes, up to a few hours.
5. **API Keys → Create API key**, twice, both with **Sending access** limited to `laztty.com`:
   - `class-manager-api`, for the API.
   - `gmail-send-as`, for Gmail.

   Each key is shown once; one key per use means revoking one never stops the other.

### 3. The API sends from `no-reply@laztty.com`

`$resourceGroup` and `$containerApp` are the names from the [pilot deployment](pilot-deployment.md#1-names-and-sign-in). Resend's SMTP user is always `resend` and the password is the API key, so `Email__Smtp__FromAddress` sets the sender (without it the API sends from the user name). `SmtpEmailTransport` connects with `SecureSocketOptions.Auto`, so port 587 negotiates STARTTLS.

```powershell
az containerapp secret set --name $containerApp --resource-group $resourceGroup --secrets "smtp-password=<class-manager-api key, re_...>"
az containerapp update --name $containerApp --resource-group $resourceGroup `
  --set-env-vars "Email__Smtp__Host=smtp.resend.com" "Email__Smtp__Port=587" `
  "Email__Smtp__UserName=resend" "Email__Smtp__Password=secretref:smtp-password" `
  "Email__Smtp__FromAddress=no-reply@laztty.com" "Email__Smtp__FromName=Laztty"
```

Updating the secret alone doesn't restart the app; the `update` creates a new revision that reads it.

Check it: **¿Olvidaste tu contraseña?** on the web app sends an email from `no-reply@laztty.com`. In Gmail, **Show original** says `SPF: PASS`, `DKIM: PASS` with domain `laztty.com` and `DMARC: PASS`. Resend's **Emails** page lists it as delivered.

Rolling back is running the [Gmail commands](pilot-deployment.md#email) again, plus `--remove-env-vars Email__Smtp__FromAddress`.

### 4. Replying from Gmail as `hola@laztty.com`

1. In the Gmail account that receives the forwarded mail: **Settings → See all settings → Accounts and Import → Send mail as → Add another email address**.
2. Name `Laztty`, address `hola@laztty.com`, untick **Treat as an alias**, **Next step**.
3. SMTP server `smtp.resend.com`, port `587`, user name `resend`, password the `gmail-send-as` key, **Secured connection using TLS**.
4. Gmail sends a code to `hola@laztty.com`; Email Routing forwards it to the same inbox. Enter it.
5. Optional: under **When replying to a message**, choose **Reply from the same address the message was sent to**.

Check it: a new email with `hola@laztty.com` picked in **From** arrives at another account without "via gmail.com" next to the sender.

## Audit commands

```powershell
Resolve-DnsName laztty.com -Type MX
Resolve-DnsName laztty.com -Type TXT
Resolve-DnsName send.laztty.com -Type MX
Resolve-DnsName send.laztty.com -Type TXT
Resolve-DnsName resend._domainkey.laztty.com -Type TXT
Resolve-DnsName _dmarc.laztty.com -Type TXT
az containerapp show --name $containerApp --resource-group $resourceGroup --query "properties.template.containers[0].env[?starts_with(name, 'Email__')]" --output table
```

Expected: the apex `MX` points only to `route*.mx.cloudflare.net`, the apex has a single SPF record, `send` points to Amazon SES, the DKIM and DMARC records exist, and the API's `Email__Smtp__Host` is `smtp.resend.com` with `Email__Smtp__FromAddress` set to `no-reply@laztty.com`. In Cloudflare, **Email Routing → Overview** shows **Enabled** and both addresses; in Resend, the domain shows **Verified**.
