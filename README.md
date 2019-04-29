# hummingbird
Tools for linking Rhino-Grasshopper with Revit.
Updated 2019-04-29 Mario Guttman

TO GET COMPILED INSTALLATION FILE AND SAMPLE DATA:
Use the content of the "_Downloads" folder.

DISCLAIMER:

I don't know how to use GitHub very well and I'm not an active member of the GitHub community so there are going to be lots of issues with this site.

OVERVIEW:

This application "Hummingbird" consists of a set of components for Rhino-Grasshopper and an add-in for Revit.  

Hummingbird is used to create native Revit objects based on data that is generated from Rhino-Grasshopper.  The Grasshopper components write to a .CSV text file that can be read by the Revit add-in, which builds the Revit objects.  The text file can be viewed in a Hummingbird CSV-Viewer (or Excel) for study and editing data if necessary, however this is not normally required.

The Rhino-Grasshopper components also include an Input tool, which can be used to read CSV data that has been created in Revit or another source.  This data is used to create Rhino geometry or data input to other Grasshopper components.

DEVELOPMENT NOTES:

This solution has historically been developed in parallel with a set of Revit tools (The WhiteFeet Tools for Revit) which has some overlapping functions.  With this GitHub version we are going to separate the two products so that they are developed independently.

One of the complex issues with maintaining this product is that it needs to be compatible with a specific version of Revit and Rhino.  We are starting this effort with Revit 2019 and Rhino 6 (which now includes the "Grasshopper" add-in natively.)  Immediately this raises the question of whether this GitHub repository should be version specific, or if we should handle the versions with branches within the repository.  For now we are just creating the single repository and will consider how to handle this issue next year.
 