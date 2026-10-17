# Repeated emails before removing the people copies

Date: 2026-10-17

Since step 5 of [users per business](backend/20261017-users/plan.md), an email belongs to one person in each business (`IX_Users_TenantId_Email`, unique). Before that, clients, students and instructors kept their own copy of the email and could repeat it. The migration `RemovePeopleCopies` stops with an error that names [`scripts/list-repeated-emails.sql`](../scripts/list-repeated-emails.sql) while any business still has a repeated email, so the deploy that brings it fails until they are fixed.

## Before deploying

1. Azure portal → the SQL database → **Query editor**, and sign in with your Microsoft Entra account (the same steps as [changing a subscription](changing-a-subscription.md#running-it)).
2. Run the whole script. Each row is one person with a repeated email: the business, the email, the name, and where they show up (`Client`, `Student`, `Instructor`, `Team` or `Brand owner`).
3. Fix each repetition in the app, from the business's account:
   - The same person loaded twice (a contact also loaded as an instructor, a student loaded in two families): keep one and change or clear the email of the other.
   - Two people sharing an email (siblings with a parent's email): leave the email on the person who uses it and clear it on the others.
4. Run the script again until it returns no rows, then deploy.

If the deploy already failed, the migration rolled back and the database is unchanged: fix the repetitions and run the deploy again.
