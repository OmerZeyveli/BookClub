# Book Club App

A Unity-based mobile application designed for a real-life book club managed by **İyilik Derneği (iyi-der)**.  
Users can sign up, log in, and select books from an in-app list.  
All user data (except credentials, which are not shared in this repository) is stored in **AWS DynamoDB**.

---

## Table of Contents
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Setup and Installation](#setup-and-installation)
- [Privacy Policy](#privacy-policy)
- [License](#license)
- [Credits](#credits)

---

## Features
1. **Sign Up / Log In**  
   - Users create an account with an email and a secure password (hashed + salted in DynamoDB).
2. **Select Books**  
   - Allows users to browse and pick books from an available list.
3. **Admin Portal (Managed by iyi-der)**  
   - iyi-der administrators view which users selected which books in real time (portal code not included here).

---

## Tech Stack
- **Unity (C#)**  
  - Main development environment for the client-side application.
- **AWS DynamoDB**  
  - NoSQL database storing all user data securely.
- **AWS SDK for Unity**  
  - Used to interact with AWS services (not included in this repo for credential security).
- **.NET / C#**  
  - Primary programming language within Unity.

---

## Setup and Installation

1. **Clone or Download the Repository**
   ```bash
   git clone https://github.com/yourusername/BookClubApp.git
2. **Open in Unity**
   - Use Unity version 6000.0.29f1 (other unity 6 versions may also work) to open the BookClubApp folder as a Unity project.
3. **Install Required Packages**
   - From [Amazon AWS website](https://docs.aws.amazon.com/mobile/sdkforunity/developerguide/setup-unity.html), install any necessary AWS SDK packages or dependencies your project requires.
4. **Add AWS Credentials (Locally)**
   - Do not commit real AWS credentials to version control.
   - Store credentials in a secure location (e.g., environment variables, a private config file, or a secure Unity service).
5. **Build/Run**
   - Test within the Unity Editor.
   - Deploy to iOS or Android by following Unity’s build process for each platform.

---

## Privacy Policy

We value user privacy and follow best practices for data security.
Read our [Privacy Policy](https://github.com/OmerZeyveli/BookClub/blob/main/Privacy%26Policy.md) here.
(You can also access it in the app by tapping the “Privacy Policy” button.)

---

## License
This project is licensed under the terms of the GPL-3.0 license.
Please refer to the [LICENSE](https://github.com/OmerZeyveli/BookClub/blob/main/LICENSE) file for more details.

---

## Credits
- Developer: Ömer (Riive) Zeyveli
- GitHub: https://github.com/OmerZeyveli
- [İyilik Derneği (iyi-der)](https://www.iyilikdernegi.org.tr/tr): For administrating the book club and database management.
- Amazon Web Services: DynamoDB for secure data storage.
- Unity: Game engine/framework used for developing the app.
- Special Thanks: To all contributors and testers who helped make this app possible.
