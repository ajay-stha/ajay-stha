## 📱 Bajra Time Log — Mobile App

A cross-platform **.NET MAUI** mobile app (Android & iOS) that lets Bajra Technologies employees
log their work hours against tasks directly from their phones, using the company's Odoo system.

### Features
- 🔐 **Secure login** — email + password via Odoo JSON-RPC; session persisted in device SecureStorage  
- ✅ **Auto session restore** — stay logged in across app restarts until you explicitly sign out  
- 📋 **Task picker** — loads your active tasks from `bajra_scrum.task` automatically  
- ⏱ **Time log form** — date picker (today by default), task, hours, and description  
- 💬 **Inline validation** — clear error and success messages on screen  
- 🚪 **Sign out** — destroys the server session and clears local credentials  

### Getting Started

#### Prerequisites
| Tool | Version |
|------|---------|
| .NET SDK | 8.0+ |
| .NET MAUI workload | `dotnet workload install maui` |
| Android SDK / Xcode | per platform |
| Visual Studio 2022 17.8+ **or** VS Code + MAUI extension | |

#### Run on Android
```bash
cd BajraTimeLog
dotnet build -f net8.0-android
dotnet run  -f net8.0-android
```

#### Run on iOS (macOS only)
```bash
cd BajraTimeLog
dotnet build -f net8.0-ios
dotnet run  -f net8.0-ios
```

### Project Structure
```
BajraTimeLog/
├── Constants.cs               # Base URL, SecureStorage keys, model names
├── App.xaml / .cs             # Application entry, session restore on start
├── AppShell.xaml / .cs        # Shell navigation (//login ↔ //timelog)
├── MauiProgram.cs             # DI container registration
│
├── Models/
│   └── OdooModels.cs          # OdooJsonRpcRequest, OdooTask, TimeLogEntry
│
├── Services/
│   ├── IOdooService.cs        # Service interface
│   └── OdooService.cs         # Odoo JSON-RPC implementation
│
├── ViewModels/
│   ├── BaseViewModel.cs       # IsBusy / IsNotBusy base
│   ├── LoginViewModel.cs      # Login logic
│   └── TimeLogViewModel.cs    # Time log form logic
│
└── Views/
    ├── LoginPage.xaml / .cs   # Login UI
    └── TimeLogPage.xaml / .cs # Time log form UI
```

### Configuration
All connection settings live in `Constants.cs`:
```csharp
public const string BaseUrl = "https://bajratechnologies.com";
```
The database name is auto-discovered from `/web/database/list` on first login and cached in SecureStorage.

---

### 👋 Hi, I'm Ajay Shrestha

I'm a passionate developer exploring full-stack technologies and building modern solutions across **Web**, **Android**, and **Desktop platforms** using **.NET MAUI**, **C#**, and **JavaScript**.  
Currently on a journey of continuous learning, collaboration, and building meaningful tech.

<p>
    <img src="https://komarev.com/ghpvc/?username=ajay-stha&style=for-the-badge">
</p>

---

### 🛠 Technologies I Use

#### 👨‍💻 Languages
<p>
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" />
  <img src="https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black" />
  <img src="https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white" />
  <img src="https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white" />
</p>

#### 🧩 Frameworks & Libraries
<p>
  <img src="https://img.shields.io/badge/.NET%20MAUI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white" />
  <img src="https://img.shields.io/badge/React-20232a?style=for-the-badge&logo=react&logoColor=61dafb" />
  <img src="https://img.shields.io/badge/Select2-007BFF?style=for-the-badge&logo=jquery&logoColor=white" />
</p>

#### 💾 Databases
<p>
  <img src="https://img.shields.io/badge/MySQL-00758F?style=for-the-badge&logo=mysql&logoColor=white" />
  <img src="https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white" />
</p>

#### 🧪 Tools & Platforms
<p>
  <img src="https://img.shields.io/badge/Git-F05032?style=for-the-badge&logo=git&logoColor=white" />
  <img src="https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white" />
  <img src="https://img.shields.io/badge/VS_Code-007ACC?style=for-the-badge&logo=visual-studio-code&logoColor=white" />
  <img src="https://img.shields.io/badge/Figma-F24E1E?style=for-the-badge&logo=figma&logoColor=white" />
</p>

---

### 🌱 Currently Learning
- Building cross-platform apps with **.NET MAUI**
- Strengthening **frontend skills** and **UX principles**
- Working on collaborative projects and improving version control

---

### 💬 Let's Connect
- LinkedIn: [linkedin.com/in/ajay-stha](https://linkedin.com/in/ajayxshrestha)
- Portfolio: Coming Soon!
