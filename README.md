# Couchbase Explorer for Visual Studio

<p align="center">
  <img src="resources/logo.png" alt="Couchbase Explorer Logo" width="128" />
</p>

<p align="center">
  <strong>Browse, explore, and manage your Couchbase data directly from Visual Studio</strong>
</p>

<p align="center">
  <a href="#features">Features</a> •
  <a href="#installation">Installation</a> •
  <a href="#getting-started">Getting Started</a> •
  <a href="#roadmap">Roadmap</a> •
  <a href="#contributing">Contributing</a>
</p>

<p align="center">
  <img src="https://img.shields.io/visual-studio-marketplace/v/codingwithcalvin.VS-CouchbaseExplorer?style=for-the-badge" alt="Marketplace Version" />
  <img src="https://img.shields.io/visual-studio-marketplace/i/codingwithcalvin.VS-CouchbaseExplorer?style=for-the-badge" alt="Marketplace Installations" />
  <img src="https://img.shields.io/github/license/CodingWithCalvin/VS-CouchbaseExplorer?style=for-the-badge" alt="License" />
  <img src="https://img.shields.io/github/actions/workflow/status/CodingWithCalvin/VS-CouchbaseExplorer/release_build_and_deploy.yml?style=for-the-badge" alt="Build Status" />
</p>

---

> **Note:** This extension is currently in **BETA / PREVIEW**. We're actively developing new features and would love your feedback!

## Features

Couchbase Explorer brings powerful database management capabilities directly into your Visual Studio workflow:

### Connection Management
- **Multiple Connections** - Save and manage connections to multiple Couchbase Server instances
- **Secure Credential Storage** - Passwords are securely stored using Windows Credential Manager
- **SSL/TLS Support** - Connect securely to your clusters with SSL encryption
- **Connection Testing** - Verify your connection settings before saving

### Data Browsing
- **Hierarchical Tree View** - Navigate your cluster structure: Buckets → Scopes → Collections → Documents
- **Lazy Loading** - Efficient loading of large datasets with batched document retrieval
- **Document Viewer** - View document contents in a formatted JSON editor with syntax highlighting
- **Refresh Support** - Refresh any level of the tree to see the latest data

### Document Operations
- **View Documents** - Double-click or right-click to open documents in a dedicated editor tab
- **Copy JSON** - Quickly copy document contents to clipboard
- **Copy Document ID** - Copy document IDs for use in your code

## Installation

### From Visual Studio Marketplace

1. Open Visual Studio
2. Go to **Extensions** → **Manage Extensions**
3. Search for "**Couchbase Explorer**"
4. Click **Download** and restart Visual Studio

### From VSIX File

1. Download the latest `.vsix` file from [GitHub Releases](https://github.com/CodingWithCalvin/VS-CouchbaseExplorer/releases)
2. Double-click the file to install
3. Restart Visual Studio

## Getting Started

### Opening Couchbase Explorer

1. Go to **View** → **Couchbase Explorer**
2. The tool window will appear (by default, docked near Server Explorer)

### Adding a Connection

1. Click the **Add Connection** button in the toolbar
2. Enter a friendly name for your connection
3. Enter your Couchbase Server hostname or IP address
4. Provide your username and password
5. Enable SSL/TLS if required
6. Click **Test Connection** to verify
7. Click **Save**

### Browsing Data

1. Right-click your connection and select **Connect**
2. Expand the tree to browse: **Buckets** → **Scopes** → **Collections** → **Documents**
3. Double-click any document to view its contents

## Requirements

- **Visual Studio 2022** or **Visual Studio 2026** (x64 and ARM64 supported)
- **Couchbase Server** 7.0+ or **Couchbase Capella** (Capella support coming soon)

## Roadmap

We're actively working on new features! Here's what's coming:

- [ ] **Couchbase Capella Support** - Connect to cloud-hosted Capella clusters
- [ ] **N1QL Query Editor** - Write and execute N1QL queries
- [ ] **Document Editing** - Create, update, and delete documents
- [ ] **Index Management** - View and manage indexes
- [ ] **Search Integration** - Full-text search capabilities
- [ ] **Import/Export** - Bulk data operations

Have a feature request? [Open an issue](https://github.com/CodingWithCalvin/VS-CouchbaseExplorer/issues/new)!

## Contributing

Contributions are welcome! Whether it's bug reports, feature requests, or pull requests - we appreciate all feedback.

### Development Setup

1. Clone the repository
2. Install [Visual Studio 2022 or 2026](https://visualstudio.microsoft.com/) with the **Visual Studio extension development** workload
3. Install the [Extensibility Essentials 2022](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ExtensibilityEssentials2022) extension
4. Open `src/CodingWithCalvin.CouchbaseExplorer.sln`
5. Press F5 to launch the experimental instance

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributors

<!-- readme: contributors -start -->
[![CalvinAllen](https://avatars.githubusercontent.com/u/41448698?v=4&s=64)](https://github.com/CalvinAllen) 
<!-- readme: contributors -end -->

## Disclaimer

This extension is an independent, community-driven project and is **not affiliated with, endorsed by, or sponsored by Couchbase, Inc.** Couchbase and Capella are trademarks of Couchbase, Inc.

---

<p align="center">
  Made with ❤️ by <a href="https://github.com/CodingWithCalvin">Coding With Calvin</a>
</p>
