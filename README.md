<div align='center'>
<h1>Anf</h1>
</div>

<div align='center'>
<h5>A cross-platform comic reader written in Rust</h5>
</div>

# What is this

A cross-platform comic reader with a Rust backend, supporting multiple comic sources through a plugin-based engine system.

# Architecture

```
anf-rs/
├── crates/
│   ├── anf-core          # Core traits and models
│   ├── anf-easy          # Easy-to-use abstractions
│   ├── anf-know-engines  # Built-in comic engine implementations
│   ├── anf-web           # Axum web server
│   ├── anf-plugins       # Lua plugin system
│   ├── anf-resource-fetcher
│   └── anf-channel-model
└── apps/
    └── anf-tauri         # Tauri desktop app
```

# Supported Engines

| Engine | Search | Proposal | Source |
|--------|--------|----------|--------|
| Dm5 | ✓ | ✓ | dm5.com |
| DMZJ | ✓ | ✓ | dmzj.com |
| Bilibili | ✓ | ✓ | manga.bilibili.com |
| Kuaikan | ✓ | ✓ | kuaikanmanhua.com |
| Tencent | ✓ | ✓ | ac.qq.com |
| Mangabz | ✓ | ✓ | mangabz.com |
| Qimiao | ✓ | ✓ | qimiaomh.com |
| Bikabika | ✓ | ✓ | bikabika.com |
| Jisu | ✓ | ✓ | 1kkk.com |
| Xmanhua | ✓ | ✓ | xmanhua.com |
| Soman | ✓ | - | soman.com |

# Build

```bash
# Build all crates
cargo build

# Build web server
cargo build -p anf-web

# Build desktop app
cargo build -p anf-tauri

# Run tests
cargo test
```

# Run

```bash
# Start web server
cargo run -p anf-web

# Start desktop app
cargo run -p anf-tauri
```

# License

MIT
