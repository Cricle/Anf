<div align='center'>
<h1>Anf</h1>
</div>

<div align='center'>
<h5>A cross-platform comic reader written in Rust</h5>
</div>

# What is this

A cross-platform comic reader with a Rust backend, supporting multiple comic sources through a Lua plugin system.

# Architecture

```
anf-rs/
├── crates/
│   ├── anf-core          # Core traits and models
│   ├── anf-easy          # Easy-to-use abstractions
│   ├── anf-know-engines  # Built-in comic source conditions (URL matching)
│   ├── anf-web           # Axum web server
│   ├── anf-plugins       # Lua plugin system (mlua)
│   ├── anf-resource-fetcher
│   └── anf-channel-model
├── apps/
│   ├── anf-tauri         # Tauri desktop app
│   └── frontend          # Vue 3 + Vuestic UI (shared)
└── plugins/              # Lua plugin scripts
```

# Supported Engines

All search and proposal engines are implemented as Lua plugins in the `plugins/` directory.

| Plugin | Search | Proposal | Source |
|--------|--------|----------|--------|
| dm5.lua | ✓ | ✓ | dm5.com |
| jisu.lua | ✓ | ✓ | 1kkk.com |
| mangabz.lua | ✓ | ✓ | mangabz.com |
| xmanhua.lua | ✓ | ✓ | xmanhua.com |
| kuaikan.lua | ✓ | ✓ | kuaikanmanhua.com |
| tencent.lua | ✓ | ✓ | ac.qq.com |
| bilibili.lua | - | ✓ | manga.bilibili.com |

Bilibili search API is currently broken server-side (returns error 99).

See `plugins/example.lua` for the Lua plugin API reference.

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

# Test engines
cargo run --example test_engines
```

# Run

```bash
# Start web server (port 5000)
cargo run -p anf-web

# Start desktop app
cargo run -p anf-tauri
```

# License

MIT
