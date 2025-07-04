# Video Storage Directory

This directory contains uploaded video files for courses.

## Structure:
- Video files are stored with unique names: `originalname_timestamp_guid.extension`
- Supported formats: mp4, avi, mkv, mov
- Maximum file size: 100MB (configurable in appsettings.json)

## Access:
- Videos are served via `/videos/{filename}` endpoint
- Static file serving is configured in Program.cs

## Security:
- Only admins can upload/delete videos
- File validation includes size, extension, and MIME type checks
- Unique file names prevent conflicts and direct access guessing
