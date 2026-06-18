# GFI_Upgrated Session Log

## Workspace Setup
- Created a separate rewrite workspace at `D:\GFI\GFI_Upgrated`.
- Kept the existing `GFIWEBSITE` and `GfiApi` projects untouched and using them only as reference.
- Created the initial solution and project scaffold:
  - `GFI_Upgrated.Data`
  - `GFI_Upgrated.SharedDto`
  - `GFI_Upgrated.ServiceApi`
  - `GFI_Upgrated.UI`

## Architecture Direction
- Chosen shape for the rewrite:
  - Data layer for persistence only
  - Service/API layer for business rules and endpoints
  - Shared DTO layer for contracts
  - UI layer for Blazor components and API calls
- Module-first organization will be used inside the solution:
  - `Admin`
  - `Acc`
  - `Store`
- The first migration target will be the `Admin` module.

## Dependency Notes
- UI project is being prepared as a Blazor WebAssembly application for a cleaner component-based frontend.
- MudBlazor is intended for the UI layer.
- EF Core and ASP.NET Core API will be used for the new backend stack.
- Dependency additions will be tracked here as they are introduced.

## Migration Notes
- Admin folders were seeded in each layer to start the module migration cleanly.
- The new rewrite will be built from scratch rather than modifying the old WebForms application.
- The old application remains the reference source for behavior, screens, and business rules.

## Scaffold Cleanup
- Removed the default template sample pages from the UI project.
- Replaced the sample API weather endpoint with controller-based routing.
- Added the first Admin placeholder screen and Admin API controller to anchor the rewrite.

## Verification
- Restored the full `GFI_Upgrated` solution successfully.
- Built the solution successfully with zero errors.

## Admin Security Foundation Migration
- Added shared DTOs for admin login, roles, users, menus, role permissions, and user-role assignment workflows.
- Added a SQL-backed admin security repository in the data layer that mirrors the legacy stored procedure shapes for login, role, user, menu, permission, and staff lookup operations.
- Added a thin admin security service and controller layer in the API project to expose the new rewrite endpoints under `api/admin/security`.
- Added a Blazor WebAssembly admin shell with MudBlazor pages for login, role management, user management, menu management, and role-permission mapping.
- Added a UI session state container so the new login response can drive menu visibility and the admin shell.
- Added lightweight admin-page guards so unauthenticated users are redirected to login and the admin menu stays hidden until a session is present.
- Added a new API base URL configuration file in the UI project for the separate UI/API deployment model.

## Validation Notes
- `GFI_Upgrated.SharedDto` built successfully.
- `GFI_Upgrated.Data` built successfully offline with `--no-restore`.
- `GFI_Upgrated.UI` built successfully offline with warnings only.
- `GFI_Upgrated.ServiceApi` is wired up in code, but full build validation in this sandbox is being limited by the local .NET SDK workload resolver / offline restore environment.

## Legacy Theme Adoption
- Mirrored the legacy `Website/assets` theme into the new UI workspace so the rewrite can use the same admin styling locally.
- Replaced the default Blazor/MudBlazor app chrome with a theme-based admin shell that follows the legacy horizontal layout pattern.
- Split login into a dedicated auth layout so `/login` renders without the main shell, navbar, or drawer.
- Updated the home page to a theme-aligned landing card so the public entry point matches the new shell direction.

## Startup Crash Fix
- Restored the `api-config` bootstrap element in the new UI host page so the legacy `common.js` script no longer throws on startup.
- Added a safe default API base URL bootstrap value before the legacy scripts load so the app can start even if local storage is empty.

## Legacy Redirect Cleanup
- Replaced the remaining `/Login.aspx` runtime redirects in the copied legacy JS with the Blazor route `/login`.
- Kept the new workspace free of WebForms login navigation so the Blazor UI no longer depends on a missing ASPX page.

## Legacy JS Removal
- Removed the copied legacy startup scripts from the new UI host path so the app no longer boots through old WebForms behavior.
- Kept only the admin theme styling assets needed for the shell, and stripped the old jQuery plugin imports from the copied theme stylesheet.

## User Role Continuation
- Added a new Admin user-role assignment screen in the Blazor UI so the rewrite can manage login-to-role mappings without falling back to legacy JS.
- Wired the new screen into the admin navigation and dashboard so it is part of the normal Admin flow.
- The new screen uses the existing `GetUserRolesAsync` and `SaveUserRolesAsync` API surface and loads logins plus roles from the rewrite service layer.

## API Diagnostics
- Added Swagger UI to the Service API project so the backend can be inspected directly in the browser.
- Updated the Service API launch profile to open the Swagger page automatically.
- Added a lightweight `/health` endpoint and a root redirect to `/swagger` to make API startup easier to verify.
- Pinned the API output to `Microsoft.OpenApi` 1.6.23 and replaced the stale 1.6.17 copy in the debug output folder after a runtime loader mismatch surfaced.

## Login Hardening and Layout
- Fixed the login repository calls so `@IPAddress` and related metadata never go in as null values during login or user save operations.
- Updated the Blazor login page to send safe client metadata before calling the API, which avoids the SQL insert failure on `Z_UsersLoginsLog`.
- Reworked the login screen into a centered full-viewport layout so it now fills the screen instead of showing as a half-width split panel.
- Changed the root home route to redirect unauthenticated users to `/login`, keeping the login page as the default entry point.

## Login Client Fix
- Fixed the UI API client to deserialize the login envelope with web-style JSON options, so the `success/data` response from the API is recognized correctly.
- Restored the login page to the intended split-screen auth layout with the image on one side and the login form on the other for large screens.

## Loader And Redirect Fix
- Removed the Blazor startup loading placeholder from `wwwroot/index.html` so the UI no longer shows a boot loader.
- Changed the post-login navigation to a normal client-side route change so the singleton session state is preserved and the app does not bounce back to `/login`.

## Vuexy-Style Shell
- Reworked the main app shell into a dark left sidebar plus top search bar layout to match the Vuexy dashboard reference.
- Rebuilt the admin dashboard with KPI cards and larger report panels so the landing view now follows the same overall composition as the screenshot.
- Removed the remaining forced reload redirects in the auth-gated pages so the login flow stays on the client side.

# Session Log

## Date
2026-05-11

## Completed
- **Complete RBAC System Implementation**:
    - **Database Extensions**: Added Export, Print, and Approve permission columns to `Z_UsersMenu` and `Z_UsersRoleForm`.
    - **Shared DTOs**: Updated `MenuDto` and `RolePermissionDto` to support extended permissions and JWT tokens.
    - **JWT Authentication**: Implemented JWT Bearer authentication in the Service API. Updated `SecurityController` to issue signed tokens.
    - **API Security**: Created `RequirePermissionAttribute` for declarative endpoint protection.
    - **Route Discovery**: Added `discover-routes` endpoint to automatically populate the menu/page database from API descriptors.
    - **UI Security**:
        - Updated `AppSessionState` to manage JWT and extended permissions.
        - Updated `AdminSecurityApiClient` to include Bearer tokens in all requests.
        - Created `AuthorizeAction` Blazor component for granular button/UI element control.
        - Updated `Permissions.razor` to manage new permission types and enforce action-level security.
    - **Role Assignment Fix**:
        - Fixed a critical sync issue where `SaveUserRolesAsync` only updated mapping tables but not the primary `Z_UsersLogins` role reference.
        - Implemented explicit SQL sync to update `Z_UsersLogins.RoleID` with the designated "Default" role.
        - Added automatic session refresh in the UI if the current administrator updates their own roles.
    - **User Role Preloading & UI Enhancements**:
        - Updated `UserDto` and `SaveUserRequest` to include `RoleId`.
        - Enhanced `AdminSecurityRepository.MapUser` to automatically fetch and populate the `RoleID` (primary/default role).
        - Updated `Users.razor` to include a "Primary Role" dropdown that pre-selects the user's current role.
        - Synchronized `SaveUserAsync` to update role mappings if a role is selected in the User Edit panel.
        - Added deep-linking support to `UserRoles.razor` via `LoginId` query parameter.
        - Added a "Roles" quick-link in the User list for rapid RBAC management.
    - **Dashboard Column Fix**:
        - Resolved a 500 Internal Server Error: `Invalid column name 'DashboardPage'`.
        - Confirmed that `DashboardPage` does not exist in the current `Z_UsersRoles` table.
        - Removed the invalid column from the SQL `SELECT` in `GetLoginResultAsync` and the `UPDATE` in `SaveRolePermissionsAsync`.
        - Implemented dynamic dashboard path discovery by searching for the first authorized dashboard in the menu tree instead of relying on a hardcoded column.
        - Hidden the dashboard selection UI in `Permissions.razor` to maintain alignment with the database schema.
    - **Permission Save Fix**:
        - Resolved a 500 Internal Server Error when saving permissions.
        - Identified that `Z_UsersRoleForm` table only supports `View`, `Insert`, `Update`, and `Delete` permissions.
        - Removed `Export`, `Print`, and `Approve` columns from the SQL save/load logic and the UI to match the strict DB-First schema.
    - **Super Admin Bypass**:
        - Implemented robust bypass logic in `RequirePermissionAttribute` (API) and `AppSessionState` (UI).
        - Users with `IsAdmin = true` or Role names "SuperAdmin"/"Admin" now automatically bypass all permission checks.
    - **Enhanced Route Matching**:
        - Updated path normalization (ToLower, Trim, Slash replacement) to ensure consistent matching between DB paths and actual routes.
    - **Debug Logging**:
        - Added `System.Diagnostics.Debug` logging to the API authorization filter to track access decisions (Role, Page, Action, Result).
    - **Bug Fixes**: 
        - Resolved `CS0234` error by installing `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package.
        - Resolved `CS0246` error by adding missing `using GFI_Upgrated.UI.State;` in `AdminSecurityApiClient.cs`.
        - Fixed missing `app.UseAuthentication()` and `app.UseAuthorization()` in `Program.cs` middleware pipeline.

## Database/API Changes
- **SQL Scripts Provided**:
    - `ALTER TABLE Z_UsersMenu ADD IsInsert BIT, IsUpdate BIT, IsDelete BIT, IsExport BIT, IsPrint BIT, IsApprove BIT;`
    - `ALTER TABLE Z_UsersRoleForm ADD ExportPer BIT, PrintPer BIT, ApprovePer BIT;`
- **New Endpoints**:
    - `POST api/admin/security/discover-routes`: Discovers and upserts API routes into the database.

## Files Modified
- `src/GFI_Upgrated.SharedDto/AdminSecurity/AdminSecurityDtos.cs`
- `src/GFI_Upgrated.Data/AdminSecurity/AdminSecurityRepository.cs`
- `src/GFI_Upgrated.ServiceApi/Program.cs`
- `src/GFI_Upgrated.ServiceApi/Services/AdminSecurityService.cs`
- `src/GFI_Upgrated.ServiceApi/Controllers/Admin/SecurityController.cs`
- `src/GFI_Upgrated.ServiceApi/Infrastructure/RequirePermissionAttribute.cs`
- `src/GFI_Upgrated.UI/Services/AdminSecurityApiClient.cs`
- `src/GFI_Upgrated.UI/State/AppSessionState.cs`
- `src/GFI_Upgrated.UI/Layout/AuthorizeAction.razor`
- `src/GFI_Upgrated.UI/Modules/Admin/Pages/Permissions.razor`

## Debug Logs (Sample Output)
- `[RBAC] Access GRANTED for admin@example.com to /admin/roles via Admin Bypass.`
- `[RBAC] Checking access for user@example.com to /admin/staff Action: view`
- `[RBAC] Access DENIED: Page '/admin/staff' not found in role permissions.`

## Pending
- Apply `[RequirePermission]` to all sensitive API controllers.
- Wrap all action buttons (Add, Edit, Delete, Export) across all modules with `<AuthorizeAction>`.

- The system is now 100% database-driven for role labels, permissions, and menu visibility.
- All hardcoded role strings ("Super Admin", "Administrator", "Admin") have been removed from the UI and replaced with dynamic `RoleName` from the database.
- Authorization bypass logic now relies strictly on the `IsAdmin` bit from the database, eliminating fragile string-based checks.
- Implemented dynamic dashboard redirection: users are now automatically sent to their assigned landing page after login.
- Dashboard assignments are now persisted to the `Z_UsersRoles` table during permission updates.
- JWT and Session State have been updated to carry the fresh `RoleName` and `DashboardPath`.
- **Route Synchronization**:
    - Performed a full audit of database `PagePath` values and actual Blazor routes.
    - Synchronized `Z_UsersMenu` table by removing legacy WebForms patterns (`~/`, `.aspx`).
    - Mapped legacy paths (e.g., `~/store/W_Dashboard.aspx`) to modern routes (e.g., `/admin`) directly in the database.
    - Updated `AppSessionState.GetHardmappedRoute` to match actual Razor page routes (`/admin/rolepermission`, `/admin/user-roles`).
    - The system now follows a strict Database-First approach, ensuring all configuration changes can be made directly in SQL Server without code modifications.
- **Dynamic RBAC Hardening**:
    - Removed all `IsAdmin` bypass logic from both the Frontend (`AppSessionState.cs`) and Backend (`AdminSecurityRepository.cs`, `RequirePermissionAttribute.cs`).
    - Every role, including Administrators, must now have explicit permissions defined in the `Z_UsersRoleForm` table.
    - Implemented route-level authorization in `MainLayout.razor` to block direct URL access to unauthorized pages, redirecting users to a new `/access-denied` page.
    - Created a reusable `<AuthorizeAction>` component to wrap UI elements (buttons, links) based on granular permissions (Add, Edit, Delete, etc.).
- **Database-Driven Menu UI**:
    - Extended the `Z_UsersMenu` table with `DisplayName` and `IconClass` columns to support dynamic UI configuration.
    - Sanitized legacy menu names (e.g., `_BomItem` to `BOM Item`) and stored user-friendly names in the database.
    - Mapped comprehensive Tabler Icons (`ti ti-*`) to all menu items directly in SQL Server.
    - Refactored `NavMenu.razor` to remove hardcoded switch-cases and `CleanLabel` logic, making the sidebar 100% data-driven.
    - Updated `MenuDto` and `AdminSecurityRepository` to bridge the new database columns to the UI layer.
    - Verified that menus are rendered according to the `SequenceNo` (Sort Order) defined in the database.
    - Ensured consistent parent-child alignment and icon spacing across the entire navigation tree.
    - System is now ready for an admin panel to manage icons and labels without code changes.
