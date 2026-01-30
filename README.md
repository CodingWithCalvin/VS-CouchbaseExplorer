<p align="center">
  <img src="https://raw.githubusercontent.com/CodingWithCalvin/VS-CouchbaseExplorer/main/resources/logo.png" alt="Couchbase Explorer Logo" width="128" height="128">
</p>

<h1 align="center">Couchbase Explorer</h1>

<p align="center">
  <strong>Browse, explore, and manage your Couchbase data directly from Visual Studio</strong>
</p>

<p align="center">
  <a href="https://github.com/CodingWithCalvin/VS-CouchbaseExplorer/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/CodingWithCalvin/VS-CouchbaseExplorer?style=for-the-badge" alt="License">
  </a>
  <a href="https://github.com/CodingWithCalvin/VS-CouchbaseExplorer/actions/workflows/build.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/CodingWithCalvin/VS-CouchbaseExplorer/build.yml?style=for-the-badge" alt="Build Status">
  </a>
</p>

<p align="center">
  <a href="https://marketplace.visualstudio.com/items?itemName=CodingWithCalvin.VS-CouchbaseExplorer">
    <img src="https://img.shields.io/visual-studio-marketplace/v/CodingWithCalvin.VS-CouchbaseExplorer?style=for-the-badge" alt="Marketplace Version">
  </a>
  <a href="https://marketplace.visualstudio.com/items?itemName=CodingWithCalvin.VS-CouchbaseExplorer">
    <img src="https://img.shields.io/visual-studio-marketplace/i/CodingWithCalvin.VS-CouchbaseExplorer?style=for-the-badge" alt="Marketplace Installations">
  </a>
  <a href="https://marketplace.visualstudio.com/items?itemName=CodingWithCalvin.VS-CouchbaseExplorer">
    <img src="https://img.shields.io/visual-studio-marketplace/d/CodingWithCalvin.VS-CouchbaseExplorer?style=for-the-badge" alt="Marketplace Downloads">
  </a>
  <a href="https://marketplace.visualstudio.com/items?itemName=CodingWithCalvin.VS-CouchbaseExplorer">
    <img src="https://img.shields.io/visual-studio-marketplace/r/CodingWithCalvin.VS-CouchbaseExplorer?style=for-the-badge" alt="Marketplace Rating">
  </a>
</p>

---

> **Note:** This extension is currently in **BETA / PREVIEW**. We're actively developing new features and would love your feedback!

## ✨ Features

Couchbase Explorer brings powerful database management capabilities directly into your Visual Studio workflow:

### 🔌 Connection Management
- 🔗 **Multiple Connections** - Save and manage connections to multiple Couchbase Server instances
- 🔐 **Secure Credential Storage** - Passwords are securely stored using Windows Credential Manager
- 🔒 **SSL/TLS Support** - Connect securely to your clusters with SSL encryption
- ✅ **Connection Testing** - Verify your connection settings before saving

### 🌳 Data Browsing
- 📂 **Hierarchical Tree View** - Navigate your cluster structure: Buckets → Scopes → Collections → Documents
- ⚡ **Lazy Loading** - Efficient loading of large datasets with batched document retrieval
- 📄 **Document Viewer** - View document contents in a formatted JSON editor with syntax highlighting
- 🔄 **Refresh Support** - Refresh any level of the tree to see the latest data

### 📝 Document Operations
- 👁️ **View Documents** - Double-click or right-click to open documents in a dedicated editor tab
- 📋 **Copy JSON** - Quickly copy document contents to clipboard
- 🆔 **Copy Document ID** - Copy document IDs for use in your code

## 🛠️ Installation

### Visual Studio Marketplace

1. Open Visual Studio 2022 or 2026
2. Go to **Extensions > Manage Extensions**
3. Search for "Couchbase Explorer"
4. Click **Download** and restart Visual Studio

### Manual Installation

Download the latest `.vsix` from the [Releases](https://github.com/CodingWithCalvin/VS-CouchbaseExplorer/releases) page and double-click to install.

## 🚀 Usage

### 📂 Opening Couchbase Explorer

1. Go to **View > Couchbase Explorer**
2. The tool window will appear (by default, docked near Server Explorer)

### ➕ Adding a Connection

1. Click the **Add Connection** button in the toolbar
2. Enter a friendly name for your connection
3. Enter your Couchbase Server hostname or IP address
4. Provide your username and password
5. Enable SSL/TLS if required
6. Click **Test Connection** to verify
7. Click **Save**

### 🔍 Browsing Data

1. Right-click your connection and select **Connect**
2. Expand the tree to browse: **Buckets > Scopes > Collections > Documents**
3. Double-click any document to view its contents

## 📋 Requirements

- 💻 **Visual Studio 2022** or **Visual Studio 2026** (x64 and ARM64 supported)
- 🗄️ **Couchbase Server** 7.0+ or **Couchbase Capella** (Capella support coming soon)

## 🗺️ Roadmap

We're actively working on new features! Here's what's coming:

- [ ] **Couchbase Capella Support** - Connect to cloud-hosted Capella clusters
- [ ] **N1QL Query Editor** - Write and execute N1QL queries
- [ ] **Document Editing** - Create, update, and delete documents
- [ ] **Index Management** - View and manage indexes
- [ ] **Search Integration** - Full-text search capabilities
- [ ] **Import/Export** - Bulk data operations

Have a feature request? [Open an issue](https://github.com/CodingWithCalvin/VS-CouchbaseExplorer/issues/new)!

## 🤝 Contributing

Contributions are welcome! Whether it's bug reports, feature requests, or pull requests - all feedback helps make this extension better.

### 🔧 Development Setup

1. Clone the repository
2. Open the solution in Visual Studio 2022 or 2026
3. Ensure you have the "Visual Studio extension development" workload installed
4. Press F5 to launch the experimental instance

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ⚠️ Disclaimer

This extension is an independent, community-driven project and is **not affiliated with, endorsed by, or sponsored by Couchbase, Inc.** Couchbase and Capella are trademarks of Couchbase, Inc.

---

## 👥 Contributors

<!-- readme: contributors -start -->
<!-- readme: contributors -end -->

---

<p align="center">
  Made with ❤️ by <a href="https://github.com/CodingWithCalvin">Coding With Calvin</a>
</p>
