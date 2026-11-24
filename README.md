# GBank Firebase Cloud Messaging Notification Service

Official GBank Architecture & Folder Placement implementation for Firebase Cloud Messaging in ASP.NET Core (.NET 10).

---

## 🏛️ GBank Project Architecture & Folder Placement

```
firebase-notification/
├── Config/
│   ├── firebase-adminsdk.json                     # Google Service Account Credentials
│   └── notifications-history.json                 # Notification Persistence Store
├── Enums/
│   ├── EnumFirebaseNotificationType.cs            # GBank Notification Type Categories
│   ├── EnumNotiUserType.cs                        # GBank User Types (All, Wallet, Agent, Merchant)
│   └── EnumRespType.cs                            # GBank Response Status (Success = 0, Error = 1)
├── Features/
│   └── Notification/                              # Feature-oriented Folder Pattern
│       ├── NotificationController.cs              # Route: api/information/notification
│       ├── NotificationSetUpService.cs            # Business Service with Logging & Result<T>
│       └── Models/
│           ├── NotificationModel.cs               # Domain & Listing Model
│           ├── NotificationCreateReqModel.cs      # Create Request DTO
│           ├── NotificationListRequestModel.cs    # Paged Search Request DTO
│           └── NotificationDetailReqModel.cs      # Detail / Delete / Resend Request DTO
├── Services/
│   ├── NotificationStore.cs                       # Thread-safe Persistent Data Store
│   └── Firebase/
│       ├── IFirebaseService.cs                    # GBank IFirebaseService Contract
│       └── FirebaseService.cs                     # FirebaseAdmin SDK, Multi-target Chunking & Logging
├── Shared/
│   └── Models/
│       └── Result.cs                              # GBank Result<T> Standard Response Model
├── wwwroot/
│   └── index.html                                 # Dashboard UI connecting to GBank Endpoints
├── Program.cs                                     # GBank WebApi Startup & DI Registration
└── firebase-notification.csproj
```

---

## 📡 GBank API Endpoints

Base Route: `/api/information/notification`

| Method | Endpoint | Description | Request Model | Response Model |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/information/notification/List` | Get paged notifications | `NotificationListRequestModel` | `Result<NotificationListingModel>` |
| `POST` | `/api/information/notification/Create` | Send notification via FCM | `NotificationCreateReqModel` | `Result<NotificationModel>` |
| `POST` | `/api/information/notification/Detail` | Get notification by ID | `NotificationDetailReqModel` | `Result<NotificationModel>` |
| `POST` | `/api/information/notification/Delete` | Delete notification | `NotificationDetailReqModel` | `Result<string>` |
| `POST` | `/api/information/notification/Resend` | Resend notification | `NotificationDetailReqModel` | `Result<NotificationModel>` |

---

## 🚀 How to Run

```bash
dotnet run --project firebase-notification/firebase-notification.csproj --urls "http://127.0.0.1:5100"
```

Open Dashboard:
👉 **[http://127.0.0.1:5100](http://127.0.0.1:5100)**
