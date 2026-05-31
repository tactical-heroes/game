Да, 250 строк для такого reference-файла многовато. Вот версия **до 150 строк**, но без потери ключевых правил.

# Architecture Reference: Bootstrap Layer

## 1. Purpose & Responsibility

* **Core Rule**: Bootstrap creates the application. Features provide functionality. Scenes provide content. Screens provide UI.
* **Dependency Rule**: Bootstrap knows which features exist, but not how they work. Features must not depend on Bootstrap.
* **Ownership**: Only Bootstrap creates the application runtime. No other layer may initialize or recreate it.
* **Responsibilities**: Create application lifetime, configure DI through VContainer, register modules, initialize infrastructure, configure routing, resolve startup route, open the initial screen/scene, and maintain application-wide runtime state.
* **Forbidden**: Business logic, gameplay logic, feature-specific UI behavior, and domain rules must not be placed in Bootstrap.

## 2. Startup Flow & Bootstrap Scene

* **Flow**: Unity Startup → `00_Bootstrap.unity` → `AppBootstrapper` → `AppCompositionRoot / AppLifetimeScope` → Dependency Registration → Feature Installation → Infrastructure Initialization → Startup Route Resolution → Initial Screen/Scene.
* **Scene Path**: `Assets/Scenes/Bootstrap/00_Bootstrap.unity`.
* **Build Settings**: `00_Bootstrap.unity` must be the first scene.
* **Scene Role**: Infrastructure only. Not gameplay, not main menu, not feature scene.
* **Allowed Objects**: Only application-wide objects may exist here.

Recommended hierarchy:

```text id="ml9l6w"
[Bootstrap]
├── AppLifetimeScope
├── AppBootstrapper
├── ScreenHost
├── Router
├── EventSystem
├── AudioRoot
├── LocalizationRoot
└── AnalyticsRoot
```

## 3. DI, Composition Root & Lifetimes

* **Composition Root**: `AppLifetimeScope`.
* **DI Stack**: VContainer → `AppLifetimeScope` → Feature Installers.
* **Wiring Rule**: Concrete implementations are wired only in Composition Root or feature installers.
* **Bootstrap Scope**: Bootstrap may know feature installers, but not feature internals.
* **App-Wide Registration**: Foundation services, infrastructure adapters, navigation, messaging, analytics, localization, feature installers.
* **Feature Registration**: Feature-specific DI must be delegated to feature installers.

### Application Scope

* Created during startup.
* Lives until application shutdown.
* Contains: Router, Messaging, Audio, Localization, Analytics, Authentication, Session, Global Services.

### Scene Scope

* Optional.
* Used for scene-local dependencies.
* Examples: Battle, WorldMap, Tutorial.
* Contains: Scene Services, Scene Factories, Scene Controllers.
* Destroyed when the scene unloads.

### Screen Scope

* Optional.
* Used for complex UI screens.
* Examples: Inventory, Shop, Profile, Settings.
* Contains: Screen ViewModel, Screen State, Screen Commands.
* Destroyed when the screen closes.

## 4. UI & Scene Strategy

* **Router**: Application-wide service. Opens screens/scenes, passes payloads, maintains navigation stack.
* **ScreenHost**: Global Bootstrap scene object. Creates/destroys screens, manages screen stack and modal windows.
* **UI Toolkit Rule**: UI Toolkit screens are not Unity scenes.
* **Preferred Flow**: Bootstrap → ScreenHost → Routed Screen.
* **Routed Screens**: Inventory, Shop, Profile, Settings.
* **Unity Scenes**: Use only for large runtime contexts.
* **Good Scene Candidates**: Main Menu, World Map, Battle, Tutorial.
* **Bad Scene Candidates**: Inventory, Settings, Profile, Shop.

### Scene Entry Points

* Examples: `BattleSceneEntryPoint`, `WorldMapSceneEntryPoint`, `TutorialSceneEntryPoint`.
* May initialize scene-local dependencies and references.
* Must not recreate the application or application-wide services.

## 5. Startup Routes & Preview Scenes

* **Startup Routes Location**: `Assets/Settings/StartupRoutes/`.
* **Purpose**: Deterministic production startup, fast development iteration, direct screen/scene opening.
* **Production Route**: Bootstrap → Main Menu.
* **Development Route**: Bootstrap → target screen/scene, for example Inventory, Battle, or Shop.

### Preview Scenes

* **Location**: `Assets/Scenes/Preview/`.
* **Examples**: `InventoryPreview`, `ShopPreview`, `ProfilePreview`.
* **Purpose**: UI development, designer workflow, fast testing.
* **Rules**: Preview scenes may use fake services, but must not replace Bootstrap in production runtime.

## 6. Code & Review Constraints

* **Avoid for runtime dependency resolution**: `FindObjectOfType`, global singletons, static managers, manual service location.
* **Prefer**: Dependency Injection, Lifetime Scopes, Constructor Injection.
* **Bootstrap Checklist**:

  1. Is it application-wide?
  2. Does it live until shutdown?
  3. Does it participate in startup?
  4. Does it belong to Composition Root?
  5. Is it shared by multiple features?

If not, move it to feature, scene, or screen scope.
