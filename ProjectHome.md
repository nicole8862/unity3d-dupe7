The game is playable, but not much of the supporting infrastructure is present.

It plays best at 320x480 (and it ought to work great on iPhone at that resolution).

Open 'unity3d/Assets/main.unity', enter playmode and click 'Start Game'.

### Notes ###
  1. In spite of using what the manual implies are Pro-only functions (`GL.LoadPixelMatrix`), the project appears to work fine with the free Unity3D; possibly the manual is out of date.
  1. The free Unity3D will delete all of the source control metadata (another Pro-only feature) when the project is opened. If you've checked out from subversion, it won't like that (svn needs to intermediate file operations). Probably best to 'export' the project instead, then manage your own version control locally.