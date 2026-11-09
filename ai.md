AI usage disclosure
===================

- AI tooling used (Comprehensive, but only to the best of our knowledge):
  - Claude
  - Visual Studio's built-in AI features may also have been used, to an unknown extent.
- AI has so far been used for:
  - Minor revisions and bugfixes.
  - Some meaningful feature implementations, under the direction of developers.
  - Better understanding of Archipelago conventions and the API.
  - Documentation updates.
  - Better understanding of BepInEx and Unity APIs.
  - Research into the decompiled game.
  - Debugging from logs.
  - Documentation updates.
  - Automated / AI code reviews.
    - Note: Automated review is exclusively for faster iteration, and NEVER replaces manual review, testing, etc.
- All AI based changes have been reviewed and tested by maintainers before release.
- No AI-generated media (e.g. art, sound, or music) is included.


Contributing with AI
--------------------

Contributors should keep these rules in mind, to limit what is considered acceptable AI usage.

- Acceptable:
  - Minor edits and trivial / menial updates.
  - Debugging, bug hunting and bug fixing.
  - Self-initiated AI code review.
  - Dispatching automated tests.
  - Generating minor and manually reviewed documentation updates.
  - Research into the base game / Unity contents and behaviors.
  - Features when carefully and intentionally directed by developers.
- Unacceptable:
  - AI dictating design and architecture
  - Fully undirected feature implementations
  - Being unable to describe and defend the reasoning of your proposed changes, from a code level not a feature level.
  - AI-generated media (e.g. art, sound, or music).
  - Large, sweeping AI-generated refactors.
- Developers making changes that include AI-generated content or have used AI tooling must abide by the following:
  - The pull request should describe which tools were involved, and how.
  - The pull request should update the above AI usage disclosure if any usage is not currently listed
  - You are responsible for reviewing and testing everything you submit.
  - All new behavior must be tested in the actual game.
