# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.7.0] - 2025-08-21

[Added]
- Added debugging logs in the import process. Can be enabled using the "Enable debug logs" in the Preferences.
- Add the ability to retrieve programmatically the Unity GUIDs associated to an imported asset via the new `AssetManagerClient.GetImportedAssetGUIDs` method.
- Added a new organization selector in the Asset Manager window.

[Fixed]
- Fix error when uploading assets with circular dependencies.

## [1.6.0] - 2025-07-10
[Added]
- Added support for saving search filters as views in the Asset Manager window
- Search by file extension
- Support for Virtual Private Cloud

[Changed]
- Asset Type search aligns with AM service dashboard Asset Type search
- Assets with no files can now be imported for tracking
- Search filters now persist across project and collections in the Assets tab.

[Fixed]
- Appropriate messaging when user account is switched with different Organizational access.
- Re-upload failing when embedding multiple files.
- The "Update All" button will correctly display its status based on all assets in a selected project, not only the assets visible in the scroller
- Fixed the import status resetting whenever the Asset Manager window is closed or refreshed.
- Ensure the task scheduler is never null upon making an HTTP request

## [1.5.1] - 2025-06-04
[Fixed]
- Fix `ArgumentNullException` on Unity 2022 that prevents opening Asset Manager for Unity.

## [1.5.0] - 2025-05-14
[Added]
- An overlay icon showing the Asset Manager import status can now be viewed on assets in the Project Window
- Public API for triggering import operations programmatically
- Added capacity to select and copy important values in asset details page
- Added support to erase unfrozen version when the user cancel an upload
- Add validation to the import path location to prevent user from setting it outside the project

[Changed]
- Allow assets to have dependencies on assets from other projects when not using the upload mode `Force new asset`
- User and URL metadata field now have a default value to prevent errors
- Show version label for an asset dependencies

[Fixed]
- Fix an issue where deleting an asset in the cloud and re-uploading it would make "Skip Identical" never work thereafter
- Fix to import dependencies at the specified version instead of always latest
- Fix asset details page sometime not showing the dependencies correctly
- Fix an issue where upload mode `Skip identical` would wrongly see dependencies as changed
- Fix metadata fields shrinking inconsistently in upload tab when adjusting tab size
- Metadata update that fails because of bad values have a clearer log
- Fix dropdowns not showing mixed value dash when editing multiple assets with different metadata single-select values
- Fix select all on rename field first click
- Fix filter display for selected filters count

## [1.4.0] - 2025-04-11
[Added]
- Added support for optimized and converted files. Selecting import will result in all the files being imported into the project
- Added the capacity to use the 'latest' label for asset dependencies when uploading assets in the asset manager preferences
- Added a warning message when filtering and viewing outdated local assets
- Added a setting in the preferences to disable the conflict resolution modal when importing
- Added informational text when no metadata field exists in the project when trying to add one to an asset
- Added support for displaying all linked projects in the Asset Details Page

[Changed]
- Exposed all supported Unity Editor types
- Files with no local modifications are no longer re-uploaded when custom metadata fields are added, ensuring a faster and more efficient upload process.
- Adding assets to upload via the contextual menu option no longer clears assets already in the upload page
- Remove the green progress bar once import is completed successfully
- Uploaded audio files display an audio preview on the dashboard
- Changed loading dialog when analyzing assets to upload
- Reduce the client side rate limitation of actions such as upload and download

[Fixed]
- The "Include All Scripts" option now follows the Upload Settings Dependency Mode without duplicating scripts
- "Physics Material" type filter now works
- Fixed a typo for the "Physics Material" asset type filter
- Fixed build of WebGL when using both Asset Manager for Unity and the Cloud SDKs (com.unity.cloud.assets)
- Remove newly created assets when cancelling an Upload
- Source controlled assets cannot be re-uploaded with the "force new version" upload mode
- Fixed null exception when adding or removing collections from the project hierarchy
- Fixed importing of asset versions from the Versions tab of the Asset Details panel

## [1.3.0] - 2025-02-21
[Added]
- New ability to add and edit custom metadata fields on 1 or more assets at upload
- New ability to filter by custom metadata fields from the Assets' tab
- Added a search bar in filter selection
- Added an update all button on the In Project tab to allow users to get the latest version of all their imported assets
- New default behaviour when removing an asset with dependencies from your local project is to remove the exclusive (not shared with another imported asset) dependencies alongside it.
  - Users can also remove only the parent using the drop down options or stop tracking the assets with the cloud.
- New default behaviour at reimport recommends keeping the highest version of an asset, even if the asset you are importing points to an older version.
  - This can be changed in the plugin preference
- Added support for custom version label in asset versions panel
- Added an upload status tooltip to display information about what's causing an asset to be re-uploaded (detected change on the asset or its dependencies)
- New import state icon on dependencies to quickly identify outdated dependencies

[Changed]
- Import asset status is now refreshed when switching page, switching project and selecting an asset
- Re-uploading on a specific cloud asset keeps its name and tags
- Modifying the upload file path mode will re-trigger a status check
- In reimport window, instead of having Ver. 1 -> Ver. 1, now we show _ -> Ver. 1 to let users know they don't have this asset imported.
- If no project is selected when entering Upload page, the breadcrumb show "No project selected" instead of "All Assets"
- Change "Update All To Latest" context menu option name when we are in Assets tab for "Update All Imported Assets From This Project" or "Update All Imported Assets From This Collection"
- Avoid re-uploading assets with datasets that are using UVCS.

[Fixed]
- Fix "too many requests" exception happening when staging multiple asset in the upload tab
- Fixed the upload status icon on the preview in the detail page not refreshing
- Improved import operation speed
- Too many requests exception on ImportStatus filtering and/or sorting for In Project assets.
- Reimporting an asset for which the files have been moved into other folders will keep those files in these new folders instead of moving them back to their initial location.
- Fix the errors that appeared in the console when upgrading a package
- Successfully uploaded assets are now tracked if other ones failed or have been cancelled
- Improved asset grid scrolling
- In Upload page, when an item is selected, the ignore toggle stays visible.
- Don't show no files or no dependencies messages before loading is done
- Fixed wrong dependency version displayed in the detail view
- Fix the empty page when opening the app after selecting an organization and project in project settings menu.
- Detail view not updating after changing upload settings
- Detail view not updating after including/excluding all scripts
- Order collections in project list by alphabetical order
- Messages, search bar and filter now survive domain reload
- Reduce considerably the time taken to ignore/include when a lot of asset are selected and avoid Editor to block on the process.
- Fix status not updated after remove from project
- Scrollbar offset is now saved after domain reload

## [1.2.4] - 2024-12-09
[Fixed]
- Fixed uploaded dependencies not ignoring assets outside of Assets folder

## [1.2.3] - 2024-12-04
[Fixed]
- Fixed Asset Manager window being stuck on "Awaiting Unity Hub User Session" page

## [1.2.2] - 2024-12-02
[Fixed]
- Fixed assets with recursive dependencies causing a stack overflow error

## [1.2.1] - 2024-11-27
[Added]
- Ability to include/exclude all scripts inside an asset to avoid eventual compilation errors
- Add a filter option in In Project page to filter by import status (Up to date, Outdated, Deleted (on Cloud))
- Add a sorting option in In Project page to sort by import status
- Reworked upload page to better reflect what's going to be uploaded
- Upload page will now display the amount of assets and their size before the upload is triggered by the user
- Update all to latest button in In Project page and in context menu
- Dependencies are now displaying their version and import status
- Displaying progress bar during asset's dependencies gathering (Upload)
- Added a progress bar displaying the computation of upload assets status (New Version, New Asset, and Skip)

[Changed]
- Add a character limit in the search bar to avoid freeze time in the Editor

[Fixed]
- Fixed changelogs in the version tabs not displaying properly
- Avoid duplicated dependencies in Reimport window
- Properly detect conflicts on file that has moved into another folder
- Fixed missing some dependencies (like cginc files) during upload
- Switching target project in upload page after an assembly reload not registering
- Properly fetching upload status recursively based on dependencies status
- Fixed wrong option label when asset version is different from the cloud one

## [1.1.0] - 2024-10-30
[Added]
- Ability to create, rename and remove Collections
- Unmodified assets will not create a new version when re-uploaded (newly imported asset only)
- Ability to force a new version when re-uploading an asset by setting re-upload mode to "Force New Version"

[Changed]
- Improve background tasks shown when importing
- Ignored assets styles inside the grid is now more obvious
- Improvements in the reimport window
- Rename Upload mode to Reupload mode
- Add focus visual state on focusable visual element
- Save inspector width between session
- Add margin at bottom of "Add Filter" button
- Uniformize asset's information labels with the web app
- Add extra space at grid bottom to allow collapsed settings bar to not hide GridItem on the last row
- Shrink the label instead of the dropdown button in update settings panel
- Added a full GridItem refresh on asset selection
- Add caches for thumbnail and primary type extension to increase responsiveness in grid view
- Put an error status on asset inside archived projects

[Fixed]
- Fixed 400 Bad Request thumbnail error
- Ignored assets are now more obvious
- Check if scene are dirty too when importing or uploading
- Clear progress bars when starting a new import
- Import now disabled for asset with incomplete source files
- Fixes concurrent file upload errors resulting in failed asset upload.
- Fixes background task reporting.
- Removing an imported cloud asset will not delete files that are part of another imported cloud asset
- Now caching dependencies identifiers when importing asset to fix missing dependencies in the AssetDetailsPage.
- Fix Import cancel button not working when cancelling an asset import already finished before
- Remove "Fetching download URLs" background tasks when cancelling
- Move filter popup inside its window when resizing
- Fixed cancellation exceptions when displaying dependencies in details tab.
- Ignore shift selection if there is no asset selected previously
- Implement properly the Control+ click and Command + Shift + click on Mac
- Put a "No results found" message when applying a filter lead to no results
- Don't add a unnecessary tooltip that contains the same string as the message
- Fixed InProject page removal error
- Skipped upload asset don't show a progress bar
- Upload Assets button is disable when all staged assets are skipped
- Upload status mismatch not match between gridview and inspector
- Potential blank page when opening the Asset Manager and prompt to close/reopen the Unity Hub

## [1.0.0] - 2024-09-19
[Changed]
- Now discoverable on Package Manager
- Included more tags for discoverability in Package Manager

## [1.0.0-pre.2] - 2024-09-17
[Changed]
- Disabled contextual import option if an asset contains no file
- Disabled import button in multi selection panel if at least one asset is empty (contains no file)

[Fixed]
- Sort button is only visible in state where it should
- Filter button doesn't hide when loading
- Fixed scroll down in Upload tab when an asset is ignored


## [1.0.0-pre.1] - 2024-09-16
[Fixed]
- Fixed WEbGL platform issue
- Improved perfromance of some operations


## [1.0.0-exp.5] - 2024-09-10
[Fixed]
- Fixed deserialization issue
- Fixed dependency fetching performance
- Fixed successful upload not clearing the upload page


## [1.0.0-exp.4] - 2024-09-09
[Added]
- Ability to sort grid items by name, modified/uploaded date, description, asset type and status
- Added UnityEditor AssetType

[Changed]
- Grid items are now left aligned to avoid jumping items when resizing
- Can now clear grid item selection by clicking in the grid background
- Improved upload error messaging
- Improved upload by preventing too many files from being uploaded at the same time
- Improved re-upload by preventing too many files from being replaced at the same time
- In the selected asset panel info, we are now disabling tabs that are not accessible when we are disconnected from the Cloud server
- When an import or an upload is finished, with or without success, clicking on the cancel button remove the progress bar
- Modify the format of tracked assets
- Removed Status Chip from AssetDetailsPage

[Fixed]
- Fixed error during upload not displaying in the Console
- Fixed the Upload Assets button being grayed out after assembly reload
- Fixed Background Tasks stucked in not responding state
- Fixed a bug where the number of column would be incorrect if the available width for a grid item was inferior to the actual width of the item
- Support dependencies outside asset's project
- Long period of inactiveness causes loss of interactivity and Unauthorized Error
- Remove NotFoundException on the console and avoid unfinished state
- Now checking for invalid ".." in default import location path
- If there are no assets filters are disabled
- Import buttons are now hidden in Asset Details Page when under Upload Tab
- Selection is deselected and no Asset Details pane is shown when unlinking project


## [1.0.0-exp.3] - 2024-08-21

[Changed]
- Change visibility of some types to make them private
- Updated resource path
- For now on, all preview images shown in thumbnail (grid and detail panel) will be the one from the latest version of the asset instead of the current imported version.
- Side bar width is now persistent
- Created the LoadDependencies operation to track the progress of the loading of dependencies

[Fixed]
- Fixed Grid Item selection bug
- Fixed Upload Page selection bug where all assets were always selected.
- Only hover highlight Filter button when enabled
- Drags can only be started from MouseDown or MouseDrag events error
- Fixed Collections always open on Refresh
- Fix description only showed first line
- Hide the All Assets button if one or zero project in the organization
- Hide the filter button if no organization is linked or there is no project
- Hide the role chip if no organization is linked or there is no project
- Reverted previous changes into the TabView registration
- Fixed the PageManager selection to create an AllAssetsPage when no Project selected
- Uncollapsed Collection and Project in the side bar not staying uncollapsed after assembly reload


## [1.0.0-exp.2] - 2024-08-14

[Added]
- Add an error status on asset prepared to upload that is contained file from outside the Assets folder.

[Changed]
- Made button tooltips clearer
- Scrollbar now resets to the top on search event

[Fixed]
- Disabled importing files with the same name
- Disabled importing assets with no files
- Fixed poor visibility of checked sidebar arrows
- Fixed poor visibility of tabs in Light mode
- Window not opening when no UDAM project is available
- Fixed status on deleted assets on the cloud that were imported


## [1.0.0-exp.1] - 2024-08-07

[Added]
- Added a pause state to the progression bar
- Added confidence threshold slider in settings
- New settings toggle to allow user to deactivate the auto generation of tags on upload
- Asset tag generation based on the thumbnail

[Changed]
- GridItem and Tag chips are now selected on PointerDown to match other elements of the package and editor
- Renamed GetVersionsAsync method to GetAssetDataInDescendingVersionNumberOrder
- Changed old OnGui event for UIToolkit event system
- Internal refactor to AssetDescriptor
- Disable the cancel button of the progress bar when the progression is completed
- Update settings manager package version from 1.0.2 to 2.0.1

[Fixed]
- Addressed warnings coming from packages
- Public  method GetAssetDataInDescendingVersionNumberOrder does not return an IAsset anymore (to limit Asset SDK access)
- Fixed issue with non-frozen asset upload.
- Only refreshing the extension when data not already gathered before selection
- When removing an item to the selection, do not refresh all the other items
- Cleaner foldouts implementation
- We now prevent the user from trying to upload an asset from the active scene
- Removed the speed check when dragging a grid item
- Added MessageType field to MessageData to better customize help boxes and display the right icon
- When re-uploading an asset, we now check if the associated asset was an embedded dependency before overriding the "parent" asset
- UVCS files now show up in the details panel when asset is imported


## [0.9.0] - 2024-07-10

[Added]
- Incomplete and failed files will be marked as such
- Speedtree .st assets are now displaying proper icon
- Multi selection in upload tab show the ignored/included file in the selection

[Changed]
- Upgraded Asset SDK dependency to 1.2.0
- Changed version label color to white and added colored dot next to it
- Uploaded assets will be Published instead of Draft
- Limiting upload to 5 concurrent tasks at a time
- Asset Type is now different than Other when it makes sense
- Better support for bulk import operation (when importing 20 items +)
- Finished import will display a green progress bar (similar to upload)
- Added new unit tests to StorageInfoHelpBox
- Added IsGridLevelErrorOrMessage boolean to ErrorOrMessageHandlingData
- Harmonize helpBox UI style
- Moved ActionHelpBox under the Page tab
- ActionHelpBox messaging is now specific to the Active page
- Harmonize PageManager and ProjectOrganizationProvider error events
- Improved StorageInfoHelpBox message and level
- StorageInfoHelpBox info and warning level are dismissible
- Upload operation now recovers from a ServiceException by removing any new temporary assets created in project
- Upload operations intercept ServiceException and display a customized error message based on the returned detailed error from service
- Retry button in the error is now doing a Refresh
- Internal cleanup with the Services class.
- Don't remove Refresh menu option when losing network connection

[Fixed]
- Disabled transformations during import to prevent "Pending" version.
- Changed the foldout's style to prevent project selection issues when resizing
- Improved loading of projects and collections
- Refreshing the icon and other metadata from cloud when multiselecting
- Fixed failed downloads for certain files; primarily from uvcs.
- Fixed a bug that caused nested collections to not be displayed properly
- Progress not displaying right after starting an Import
- Upload button is disable when all upload asset already exist in the cloud
- File count in detail page counting system files too
- Show In Project is now pinging the primary file
- Timeout error causing the AM4U to be blocked on "It seems there was an error while trying to retrieve organization info."
- Help box error showing only in the In Project tab
- Successful upload not taking back to the Assets page
- Loading page not showing up during a Refresh
- Initial connection status being forced to false
- Removed nested scrollviews in multi select page
- Fix cancel import

[Removed]
- UserEntitlement class and all related methods to fetch UserEntitlement from internal Cloud Services endpoints.

## [0.8.0] - 2024-06-14

[Added]
- Added a + status icon for newly added upload asset
- Drag and Drop to Import/Upload in/from Project View

[Changed]
- Default import location is now set to Assets root with no sub folder creation
- Improved thumbnail loading speed and preventing stutters during scrolling
- Downloaded thumbnails are now 180px instead of 512px

[Fixed]
- Fixed the cancellation of loading assets that leaded to see previous request in search or filtered results.
- Fixed duplication of item when scrolling to the last row
- Fixed project selection not working after an domain reload
- Thumbnails are downloaded in memory before being saved on disk for later use (previously it was saved on disk, then loaded on memory)
- The number of thumbnails loaded per frame has been set to avoid causing stutters during scrolling
- Fixed the overlay of the single and multi selection detail page
- Unresponsive project list after a domain reload
- Popup maximum size is constrained in function of window size and filter selection popups have now scrollbar
- Removing am4u_guid files usage (re-upload requires an import first)
- Fixed Show In Project regression pointing on parent folder instead of first file
- Preventing free organization from being blocked from upload
- Modifying or deleting an asset prepared for upload will now refresh the upload page
- Properly clearing grid when switching page
- Service Not Found error when upload assets on Windows using a folder hierarchy
- Only ask once for create a new destination folder

## [0.7.2] - 2024-05-29

[Fixed]
* Fixed Not Found error while uploading

## [0.7.1] - 2024-05-24

[Fixed]
* Fixed Import Settings
* Running transformation causing upload to fail and not being tracked as imported

### Known Issues
* Failing assertion on import settings path on Mac
* Asset uploaded and tracked without being frozen properly because of transformation will most likely display the "out of date" status right after uploading is finished. This is due to transformation and preview modifying the date of the asset.


## [0.7.0] - 2024-05-17

[Added]
* A new preprocessing directive "AM4U_ENABLE_ASSERTIONS" that enable assertion when set.

[Changed]
* Upload mode set to Override by default
* Show In Project will ping the first file instead of the parent folder
* Draft assets will display their version in yellow like on the web app
* Disabling Remove button while re-import is in progress
* Remove the ImportOperation.ImportAsync IAsset parameter, which was a duplicate of the IAssetData given in the constructor
* Tracking recognizes version changes and out-of-date assets will be updated to the latest version on import
* Cache location is now displayed with an absolute path in the user settings
* Removed download progress indicators from the import operation
* Upload progress indicators are no longer stick
* Improved offline mode and domain reload support

[Fixed]
* Import to action wasn't working
* Don't show import progression of -100% anymore
* Imported asset without .meta file are now properly re-uploaded (need to be imported first)
* Re-uploading a new version if an asset was not tracked properly
* Upload progress not displaying error if details panel is opened
* Normalizing path's separators in AM4U settings
* Fixed Upload files displaying Assets/ folder
* Fixed critical issue where Role and Permissions where not serialized properly
* Fixed error when cancelling Import To folder selection
* Fixed error when selecting Assets folder using Import To dialog
* Removing unused constant causing warning
* Improved search to better support underscores and other separator
* Fixed missing Reveal In Finder label in settings
* Asset files occasionally failing to re-import and being deleted.
* Fixed the import of files with no extensions
* Wrong search results when adding search words while the grid view has not finished loading.
* Fixed a bug where upload progress indicators would be duplicated.

## [0.6.0] - 2024-04-26

[Added]
* It is now possible to bulk import or bulk remove by selecting in the grid view and right click on the selection
* Added new "Default import location" setting in Preferences/Asset Manager
* Added "Import to" option when clicking the Import button
* When "Create by" and "Last edit by" chip are clicked, a corresponding filter is now created
* Show user role

[Changed]
* Ignored upload item are not less dark
* Encode in the .am4u_dep file the proper asset version
* Improved progress reporting
* Username selections in "Create by" and "Update By" are now order in alphabetic order.
* Changed cursor style when hovering over Role chip and Details panel's dashboard link

[Fixed]
* Fixed reimport by cleaning-up moved files correctly
* Asset details panel showed file sizes in base 1024. Now it is harmonized with the web version and it is base 1000.
* Collection hierarchy not correctly reconstruct
* Go To Dashboard when in All Assets page open now the right web page.
* Fix double link icon while loading in Upload page
* Fixed the retrieval of asset that was looking for asset version "1", which does not exists. Use the oldest asset version.
* Fix Add Filter availability when all filter type are used
* Update of the GridItem when finish importing
* Refactored progress code to prevent hangs when importing a large number of files
* When opening the window the first time, permissions wasn't initialized properly

## [0.5.2] - 2024-04-26

### Fixed
* Fixed for Versioning API breaking uploads

## [0.5.1] - 2024-04-03

### Added
* Displaying subshader graph icons

### Fixed
* Fixed Details Page Scrollview issues
* Fix detail panel close button overlapping
* Filter visibility when window is too tight
* Disable filter dropdown when there is no asset visible in the current page
* Fixed Loading when entering play mode
* Removed "All Assets" ProjectInfo object that was used by default.
* Fixed missing dependencies foldout bug
* Upload support for asset with circular dependencies
* Fixed a bug that was preventing project selection

## [0.5.0] - 2024-03-15

### Added
* Asset uploading to Unity Cloud
* Analytics to improve user experience
* Support for Unity 2023.3
* Support for opening assets from the Unity Cloud Dashboard

### Changed
* Static placements of UI elements on the Asset Details page
* Context menu for Assets
* Icons for state of Assets

### Fixed
* Window code optimization improvements
* Loading more than 100 assets may result in unexpected behaviour
* Slow HTTP connections
* Import progress
* HTTP errors not being revealed in console
* "Open the Asset Manager Dashboard" link does not link to the specific project
* Sometimes clicking on project does not register
* Inactive "Remove From Project" button
* Highlighting and focused state colours being the same
* Thumbnails not being updaated properly
* Updating and re-importing assets may cause data sync issues

### Known Issues
* Uploading and downloading in the same project may cause issues
* In-Project view may behave incorrectly
* Cannot cancel imports
* Uploading lighting data from a Scene causes a crash

## [0.4.4] - 2024-02-29

### Changed
* Look and feel of Asset Details page
* Removed re-import tooltip
* Hid description and tag when empty
* Fixed import status not updating after a re-import
* Removed Project dashboard link

### Fixed
* Window code improvements
* Multiple tags in detail view were stacked in column instead of row
* "Open the Asset Manager Dashboard" not correctly linking to current empty project

### Known Issues
* Updating and re-importing assets may cause data sync issues
* Loading more than 100 assets may result in unexpected behaviour


## [0.4.3] - 2024-02-16

### Added
* First draft of Imported Status
* Support for corrupted/deleted/forbidden imported asset
* Displaying Asset ID in Detail page view
* Filter by Unity type

### Changed
* Look and feel of GridItem highlight and selection is now more clear
* Previously imported assets will not be display in the In Project page and have to be re-imported
* Imported assets are now displayed directly instead of making a request to the backend
* Display Unity Type instead of Dashboard Asset Type
* Display extension when hovering on the Type icon

### Fixed
* GridItem highlight now behaves properly
* Unintended window refreshing
* Cases where a GridItem shows the wrong Asset Type
* In Project Page not loading if an asset was deleted
* Forbidden error that was showing sometime when after importing an asset
* Add "Loading ..." text in dropdown popup before the selections show up

### Known Issues
* Updating and re-importing assets may cause data sync issues
* Thumbnails may nto be in sync with cloud
* Files are not tracked when moved
* Loading more than 100 assets may result in unexpected behaviour


## [0.4.2] - 2024-02-09

### Fixed
* Fixed parallel downloads
* Projects are now sorted by name
* Preview files are now ignored
* Meta file are now displayed and downloaded properly

### Added
* Added infinite loading bar when fetching files download URL
* Added "All Assets" option to allow a search over all assets of an organization
* Added link to the Project in the Unity Dashboard
* Project's chip now contains the project's icon

### Changed
* Optimized HTTP calls
* Project dropdown has been changed in favor of a tree view from project to collection

### Known Issues
* "Show In Project" is not working as expected
* “Imported” icon on the file list does not show for meta files
* Loading more than 100 assets may result in unexpected behaviour

## [0.4.0] - 2024-01-22

### Fixed

* Vertically centered the listed file items in the details page

### Added

* Warning box in the details page when there are no files associated with an asset
* Refresh menu item in the window contextual menu that refreshes projects, collections and assets

### Changed

* Initial gridview loading screen no longer shows blank grid items with loading icons


## [0.3.0] - 2024-01-03

### Fixed

* Errors caused by selecting a cache location without write permissions
* Fixed forever loading screen when organisation was not set at first launch.
* Fixed message shown when search result is empty
* Fixed forever loading page when entering play mode before page was fully loaded
* Fixed null exception when removing selected asset from in project
* Fixed details page closing when removing an asset in project that is not selected
* Don't see .meta and .DS_Store in the included file.
* Fixed import of assets with special characters

### Added

* Explanatory tooltips to disabled buttons in the Asset Details Page

### Changed

* Updated versions of com.unity.cloud packages
* Default import location path
* Show in project button now highlights the asset's file in the project browser
* Search now fetches the asset and it's files all in one call
* Remove the id in the folder name after importing and manage import name conflict

## [0.2.0] - 2023-12-05

### Added

* Message to show when they have no assets in the project and a link to the dashboard
* Message to show when Organisation has no projects and a link to the dashboard

### Fixed

* Fixed Editor namespaces being used in package which was colliding with URP and HDRP projects
* Fixed height of the thumbnail in the details view
* Collections expanded/collapsed values now survive domain reload
* Fixed Included Files initial size being too small
* Fixed file information loading forever for some assets
* Made sure we got the full project list

### Changed

* "No results found" text now shows a more descriptive message depending on the page/results

## [0.1.1] - 2023-11-16

### Changed

* Moved the Resources folder under the Editor folder so they are not part of the users' builds

## [0.1.0] - 2023-11-14

### Added

* Cancelable import progress bar has been added to the grid items and asset detail page
* Refresh button next to the search bar
* Error message for when an organization is not selected in the `Project Settings -> Services` section
* Links to the Dashboard, Project Settings and Preferences
* Next page loading icon has a loading bar with informative text
* Grid item corner icon status
* Context menu actions
* Show in project allows users to quickly find the file in their project
* In Project feature allows users to see which assets are in their projects
* Remove from project
* Import into project
* Search bar functionality
* Cache management feature allows users to manage and clear their cache
* Cache settings allows users to set the cache size limit and location of their cache
* Breadcrumbs
* Project selection dropdown allows users to select which project they want to search in
* Inspector section for the asset details page
* Collections
* Grid items
