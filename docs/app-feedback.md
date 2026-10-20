# App feedback

Ajustes → **Enviar comentarios** lets anyone on the team write a message to the people who build the app. It is how the pilot reports what doesn't work or what they miss.

## How it works

- The screen has one required field (the message, up to 4000 characters) and **Enviar**. Sending shows "Gracias, lo recibimos." and goes back; a failure keeps the message and shows a banner.
- `POST /api/me/app-feedback` (`RequireAnyAccount`, the authentication rate limit) runs `SendAppFeedbackUseCase`, which emails the message to the address in `AppFeedback:Recipient`.
- The email comes from the usual sender (`Email:Smtp`) with **Reply-To** set to the sender's account email, so answering from the inbox reaches them directly.
- Besides the message, the email lists the branch name, the sender's email, their role (`BranchOwner`, `Instructor`, `Viewer`, `Custom`, `BrandOwner`, or `StudentApp` when it comes from a student account) and the platform and app version.
- The command log keeps the message masked (`[AuditPersonal]`); nothing is stored in the database.

## Configuration

`AppFeedback:Recipient` in `src/Api/appsettings.json` is `classmanager.noreply@gmail.com`, the same Gmail account that sends the app's emails, so feedback is read there. Override it with the `AppFeedback__Recipient` environment variable to send it elsewhere. The API throws when the setting is empty, so a missing value shows as a failed request instead of feedback that silently goes nowhere.

## Not included

- Error reporting (crashes and failed requests nobody reports) is a separate step, still pending in the [pilot plan](pilot-df-swimming.md#also-needed-before-the-pilot-starts).
- The student app has no entry yet; the endpoint already accepts student accounts.
