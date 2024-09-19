# Publishing

This doc will cover the standard release process that UWB follows.

The steps are listed in order.

## Versioning

The UWB project follows semantic versioning. The general version number should be MAJOR.MINOR.PATCH. For preview builds, attach `-preview.x` to the end.

Engine packages have an additional version number attached as well. This version number is the version of the browser engine they are using (E.G: CEF engine has CEF's version attached, so might end up looking like `2.0.0-106.1.0`).

The packages and assemblies will need to have their version numbers bumped depending on what has been done to them.

## Changelog

The `CHANGELOG.md` file should generally be maintained as dev work is done, but most of the time it is not (because we are lazy), so fill out what has been done. This generally isn't too much of an issue if git commits are descriptive enough.

The date of the release should be attached next to the version number.

## Merge to Release Branch

A PR to the `release` branch should be created. Follow standard PR review process to potentially catch any last minute changes that need to be done.

## Release Build

Once merged to the `release` branch, wait for CI to complete build. The CI will output the different packages compiled as artifacts. Downloads the artifacts as they will need to be pushed.

## Push to VoltUPM

Use npm to push to VoltUPM.

> [!NOTE]
> 
> Cloudflare does not allow uploads of 100MB or larger. Some packages are larger. Add VoltUPM's server ip to `/etc/hosts`, then push. Ensure VoltUPM's server firewall has whitelisted IP from where push will be done.

## Git Tagging

Tag the commit in the `release` branch with the version number. Create GitHub release targeting this tag.
