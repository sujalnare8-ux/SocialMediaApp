![CI](https://github.com/sujalnare8-ux/SocialMediaApp/actions/workflows/ci.yml/badge.svg)

# Social Media App

A full-stack social media app with real-time chat, built to showcase scalability, real-time features, and cloud skills.

## Features
- User registration and login (ASP.NET Core Identity + JWT)
- Profiles with bio, follow and unfollow
- Posts with image uploads (Azure Blob Storage)
- Likes and comments
- Following feed and global feed
- Real-time chat (SignalR)
- Logging and health monitoring (Serilog, /health endpoint)
- CI with GitHub Actions

## Tech Stack
ASP.NET Core Web API, React (Vite), EF Core, SQL Server, Azure Blob Storage, SignalR, Serilog

## System Metrics (local)
- Health endpoint: GET /health
- Feed load (GET /api/posts/following): ~5-20 ms
- Profile posts (GET /api/users/{id}/posts): ~3 ms

## Run locally
1. Start SQL Server Express and Azurite
2. API: cd SocialMediaApp.Api, then dotnet ef database update, then dotnet run
3. React: cd socialmediaapp-client, then npm install, then npm run dev
4. Open http://localhost:5173
