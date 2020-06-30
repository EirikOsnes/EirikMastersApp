# EirikMastersApp

# Context
This program is the source code for my masters thesis. It is assumed that said thesis is read to understand the variables and usecases of the program.

# Versioning
The application is developed using Unity version 2019.2.8f1 and was working for Oculus Quest build 16.0. It is likely to work in later versions of both programs, but this remains untested.

# Download instructions
In order to access the application git, git LFS (Large File Storage) and unity must be downloaded and installed.
* Git: 	https://git-scm.com/downloads
* Git LFS: https://git-lfs.github.com/
* Unity: 	https://unity3d.com/get-unity/download (Download Unity Hub)

**NOTE:** Git LFS breaks if the repository is downloaded, meaning all assets in the application breaks. To avoid this the repository MUST be cloned.
# Assets
In accordance with their respective licenses, all environmental assets, as seen in Figure 14 have been removed from the repository. 
All assets used for this experiment can be found on:
* https://3dwarehouse.sketchup.com/model/01ae568b-4bef-4d23-b5b4-6738e3ab9b90/Deciduous-Trees-with-Realistic-Shadow
* https://free3d.com/3d-model/bus-setia-negara-low-poly-557005.html
* https://free3d.com/3d-model/automobile-v1--84248.html
* https://www.mrcutout.com/78-cutouts/people-cutouts/6068-woman-with-a-smartphone-shopping-0008
* https://www.mrcutout.com/78-cutouts/people-cutouts/7317-businessman-with-a-smartphone-walking-0012
# Overview
The application is developed using Unity, with some functionality ran through the Unity editor, and some governed by scripts (C#). For the purpose of creating and running tests – granted no functionality has broken due to software updates etc. – only the editor should be necessary, as long as tests are limited to the available variables. For further modification scripts must be created or edited. All scripts are documented in code and located under Assets/Scripts.
# Creating tests
After opening the project in Unity, the hierarchy will look like this:

![Imgur](https://i.imgur.com/yx2kNXU.png)

## Manual creation:
Selecting “Tests” should open the following view in the inspector:

![Imgur](https://i.imgur.com/ewg6zMt.png)

By changing variables here as wanted, a single test can be created. Do note that Test Set must be set carefully if ordering based on test sets are to be done. If Field of View is set to “Both”, a Narrow AND a Full test will be created. If “Randomize Quadrant” is checked, “Quadrant” is ignored. “Quadrant” is also ignored for Narrow cases. “Event Camera” and “LaserPointer” should not be changed.
The test is created when “Generate Test” is pressed, at which point buildings will spawn in the view, and one test is listed in the hierarchy like so:

![Imgur](https://i.imgur.com/cDVEnW5.png)

## Runtime Creation
Selecting “RuntimeManager” opens in the Inspector a list of components necessary for the running of tests: 

![Imgur](https://i.imgur.com/d2fGZgK.png)

The “Run Time Test Creator” is responsible for the creation of tests at runtime. It can take in any number of TestParameters, for which tests will be created. TestParameters feature largely the same options as when creating tests manually. They can be created by right-clicking in the Project view – for example in the Test Parameters folder. In the pop-up menu shown, select Create > ScriptableObject > TestParameter after which options appear in the Inspector. The test parameters utilised in the paper is available, and for height look like so:

![Imgur](https://i.imgur.com/wOIaITw.png)

Note that each TestParameter is capable of creating tests for only one active variable (Test Type), but can create any number of spans, determined by “Test Values”. 
Once a TestParameters object is created, it can be dragged and dropped onto the “Run Time Test Creator” component or selected by clicking the little circle to the right of the elements in the list.
Creating tests this way does not create any objects in the editor, but rather only at runtime.
# Changing target model
Selection works by having a canvas (an invisible plane) in front of each building that reacts to the LaserPointer and button clicks. As such to change models, a prefab (prefabricated model) must be created that includes such a canvas. This is done easiest by modifying the existing model.
The model used currently can be found under Assets > Models > Buildings > Building with Canvas Prefab. By selecting this, and dragging while holding CTRL, a copy can be created. By then double-clicking this copy, the scene changes to edit this prefab. 
Import the wanted model by dragging it into the scene or the hierarchy. Then select it and set its position to 0,0,0. Move the model so that the bottom is flush with the ground. It should now look similar to the image below (here using a simple cube).

![Imgur](https://i.imgur.com/n0HtXtw.png)

Select the Canvas and move and resize it to be equally wide and tall as your model, sitting in front. 
NOTE: If the Height variable is to be used as meters, the model created must be exactly 1 unit tall in Unity. For centimetres, it must be exactly 0.01 units tall, etc.

# Screens
The info screen and group select screen need not be changed to run but can be moved as wanted by selecting them in the Hierarchy view and changing their positions. The text shown on the info screen is determined in code (in the TestRunner Script).

# Running Tests
Selecting “RuntimeManager” the “Test Runner” component is shown in the Inspector. This component contains the remaining variables that are to be kept consistent over the tests.

![Imgur](https://i.imgur.com/3xtJN77.png)

“Create Test At Runtime” determines whether manually created tests or tests created at runtime should be utilised in the test pass. Note that if checked, TestParameters must have been added to the “Run Time Test Creator”, while if unchecked, tests must exist in the hierarchy. The Lockin Button describes which button is used for changing states, and if changed, new thumbnails on the controllers should be created.
If a tutorial is wanted, TestParameters are added to the “Tutorial Parameters” list, like how it is done for runtime creation.
To run the test, the application must be built onto an Oculus Quest with developer mode activated. See https://developer.oculus.com/documentation/native/android/mobile-device-setup/ for how to set up developer mode. In Unity, select File > Build Settings and make sure Android is selected. Then click “Player Settings…” and make sure Player > XR Settings > Virtual Reality Supported is checked.
Build to Quest by connecting it to the computer with an USB cable, and selecting File > Build and Run. At which point the application should open on the Quest. If no changes are to be made to the application, this only needs to be done once, after which the application can be found on the Quest in Library > Unknown Sources. 

# Retrieving data
During a test pass test results are saved to file as tests are completed, and once all tests are completed, a file containing the results of the full test pass is saved. By connecting the Quest to the PC these files can be located in Android/data and selecting the application’s folder. All tests are stored in the Tests folder, where each test pass is saved under the year, month, day, and time of the test beginning. All data are saved in JSON files containing all the data shown in Table 6.
