All assemblies that are depended upon by this application should be added as embedded resources in this folder (Dependencies).

To ensure that the latest version of these assemblies are embedded into this application exe, you need to add a pre-build event
that copies the assembly into this folder. To do this:
1. open the properties page for this project and open the "Build Events" tab
2. add a "copy" command to the pre-build command line to copy the assembly that this app depends on into this folder (Dependencies)
3. build the project
4. 'Include' the assembly that should now be appearing in the Dependencies
5. Set the build action of the assembly that you have just included to 'Embedded Resource'
6. Make sure you DO NOT check in these dll's into vault. This may mean that you have to remove the 'Add file' operation from
   your pending change set.