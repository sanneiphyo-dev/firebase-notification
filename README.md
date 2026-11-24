# Firebase Notification CRUD Web Tester

A modern, clean, and lightweight ASP.NET Core Web Application to test and manage Firebase Cloud Messaging (FCM) push notifications with a complete CRUD web interface.

---

## 🌟 Features

- **Create (Send):** Send notifications to **Topics** (e.g., `test-topic`, `Announcement`) or specific **Device FCM Tokens** with optional JSON payloads.
- **Read (History):** Real-time table of all sent notifications with delivery status (`Success` / `Failed`), timestamps, target badges, and Google FCM `Message ID`.
- **Resend:** One-click resend button to re-deliver any past notification.
- **Delete:** Remove individual history records or clear the entire history.
- **Desktop Popup Notifications:** Built-in Web Notification API button. When enabled, every sent notification pops up as a native Windows desktop notification toast in your browser!

---

## 📂 Project Structure

- [`firebase-adminsdk.json`](file:///C:/Users/User/source/repos/firebase-notification/firebase-notification/firebase-adminsdk.json): Google Firebase Service Account Key.
- [`Models/NotificationRecord.cs`](file:///C:/Users/User/source/repos/firebase-notification/firebase-notification/Models/NotificationRecord.cs): Record and DTO models.
- [`Services/NotificationStore.cs`](file:///C:/Users/User/source/repos/firebase-notification/firebase-notification/Services/NotificationStore.cs): Thread-safe local JSON persistence store.
- [`Program.cs`](file:///C:/Users/User/source/repos/firebase-notification/firebase-notification/Program.cs): ASP.NET Core Minimal API with FirebaseAdmin SDK integration.
- [`wwwroot/index.html`](file:///C:/Users/User/source/repos/firebase-notification/firebase-notification/wwwroot/index.html): Responsive Tailwind CSS Single-Page Application (SPA) dashboard.

---

## 🚀 How to Run

```bash
dotnet run --project firebase-notification/firebase-notification.csproj --urls "http://127.0.0.1:5100"
```

Open your browser at:
👉 **[http://127.0.0.1:5100](http://127.0.0.1:5100)**
