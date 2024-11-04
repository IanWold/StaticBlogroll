<div align="center">

<img src="img/logo.png" width="75" alt="Logo" />
    
# StaticBlogroll

<a href="https://github.com/IanWold/StaticBlogroll/releases"><img src="https://img.shields.io/github/v/release/ianwold/staticblogroll?include_prereleases&style=for-the-badge&label=version" /></a>
<a href="https://github.com/IanWold/StaticBlogroll?tab=MIT-1-ov-file"><img src="https://img.shields.io/github/license/ianwold/staticblogroll?style=for-the-badge" /></a>
<a href="https://github.com/IanWold/StaticBlogroll/discussions/categories/showcase"><img src="https://img.shields.io/github/discussions-search?query=repo%3Aianwold%2Fstaticblogroll%20is%3Aopen%20category%3AShowcase&style=for-the-badge&label=blogrolls%20made" /></a>

Publish a **blazingly fast** and **super cool** blogroll with recent posts using Github Actions and Pages!

Fast Setup ⚡ Low Maintenance 💪 Extremely Dapper 😎 Statically-Generated

[Setup](#setup) •
[Configuration](#configuration) •
[Next Steps](#next-steps) •
[Contributing](#contributing)

</div>

---

I'm a big advocate that everyone should be maintaining a blogroll. Not super actively - maybe more passively - but you should definitely have one. No doubt there's dozens of blogs a year you tune into - an article here to get help on an issue, an article there to learn about a topic, articles for fun or insight or curiousity. If you need some convincing, [I wrote a bit about this](https://ian.wold.guru/Posts/publish_your_blogroll_now.html) - on my blog!

This template repo makes it _ridiculously easy_ to get up and going with a standalone blogroll that doubles as an RSS reader, displaying the most recent posts from your blogs' RSS feeds. Since it's a template repo and not a standalone app or library, it's infinitely customizable for you to make it your own.

## Setup

It's easy to set up your own blogroll using this repo, though it does include a couple steps to configure GitHub appropriately:

1. [Create a new repo using this template](https://github.com/new?template_name=StaticBlogroll&template_owner=IanWold) (follow that link or click the `Use this template` button on the top right)
2. On your new repository, go to Settings > Actions (under Code and Automation) > General, and in the section Workflow permissions:
    1. Select `Read and Write Permissions`
    2. Check `Allow GitHub Actions to create and approve pull requests`
    3. Click Save
3. Note that the very first run will have failed - this is because the above permission for GitHub Actions was not set. After granting the permission the build should work, so let's kick off a build which will generate a new branch called `gh-pages` (from which we'll deploy the site). Add an RSS feed to `Feeds` in `config.json` and commit it to test that the build works. RSS implementations can vary widely, and sometimes RSS files can be invalid. I like testing with `https://gizmodo.com/feed` because I know it's always well-formed. If the build worked, there should now be a `gh-pages` branch in your repository!
4. Now that we have the `gh-pages` branch built with our generated site, we can deploy it using GitHub Pages. On your repository, go to Settings > Pages (Under Code and Automation), and in the Build and Deployment section:
    1. Set Source to `Deploy from a branch`
    2. Select `gh-pages` as the Branch
    3. Click Save
5. Your site should deploy in a few seconds to a default URL. Navigate to this URL to ensure it deployed correctly.
6. You're done! Add all the RSS feeds you want, though be careful that invalid RSS files can cause the build to fail. Continue on this README for all the options you have for configuration, and then when you're happy share your blogroll with the world!

The only final step is to show off your fancy new blogroll: [post your blogroll in our showcases](https://github.com/IanWold/StaticBlogroll/discussions/new?category=showcase) in the discussions for this repo!

If you get stuck anywhere please feel free to [ask a question](https://github.com/IanWold/StaticBlogroll/discussions/new?category=questions) in the discussions on this repository, or search if someone's already had the same issue. I aim to respond within the next business day unless I'm out on vacation.

### Potential Errors

The way the Action workflow works is it runs the build tool then copies the output HTML files into a separate branch, `gh-pages` from which Pages deploys the site. In order to do this, Actions needs permissions to be manually configured in the repository settings (described in step 2 above). When you first create your repository, you'll notice that Actions will kick off the workflow and this first run _will_ fail because the permissions weren't set. It's important to follow through to step 3 above to commit to the main branch to trigger a rebuild. If this one fails, be sure to check that you're checking the exact setting described above.

Pages has some interesting, clever tricks up its sleeve about where it deploys your site. If you're like me, you might have a separate repo using Pages to host a personal (or some other) site with a custom URL - in my case it's [ian.wold.guru](https://ian.wold.guru). If that's the case, Pages will deploy your site to `<your-url>/<your-repo-name>`, which in my case was [ian.wold.guru/Blogroll](https://ian.wold.guru/Blogroll). This might be desirable for you, or it might not. If your DNS for your custom domain is improperly configured, your site will not show even though Actions will report that it deployed your new blogroll to `<your-url>/<your-repo-name>`. Be sure to check your DNS settings on your custom domain. For more documentation about how Pages works with custom domains, check out [Pages' documentation](https://docs.github.com/en/pages/configuring-a-custom-domain-for-your-github-pages-site/managing-a-custom-domain-for-your-github-pages-site).

## Configuration

The `config.json` file contains all the configuration for the generated site:

* `Title` is the title of the generated blogroll site, both the page title (`<title>`) as well as at the top of the navigation bar.
* `PostsToShow` is the maximum number of posts from your RSS feeds to display on the "latest" page. Set to `-1` to show all posts.
* `CssUrl` is the URL to the CSS for your page. By default this is [MVP.css](https://andybrewer.github.io/mvp/), which I recommend because of its quality in styling plain HTML. This can be a URL to any CSS served via CDN, or to set this to your own CSS file put `yourfile.css` in the `Static` directory then set this property to `./yourfile.css`. Note that if your CSS requires any modifications to the HTML, you can update the templates in the `Templates` directory.
* `Pages` contains the title and subtitle settings for the Blogroll and Latest pages, which are the headings that display at the top of each page. You can write any HTML in these properties you like!
* `Feeds` is the set of links to your RSS feeds.

## Next Steps

This is very intentionally _not_ a product you install and host, but a template for you to make your own repo and do your own thing with. It's a base canvas you can use and extend however works best for you!

### Files

* `/Static` Files in this directory are copied over to the root level of the output directory. You can include CSS, favicon, anything!
    * `Robots.txt` is the only file here by default. This controls how search engines navigate your site. By default it only includes `noindex nofollow`, which prevents search engines from listing the site.
* `/Templates` this includes the templates for the Blogroll and Latest pages, as well as a base layout which holds the containing HTML for each page.
* `build.csx` is the C# Script file that generates the static site. It reads in `config.json`, fetches and processes the RSS feeds, then uses [Metalsharp](https://github.com/IanWold/Metalsharp) to generate the site. You can edit this file to change any aspect of how the site is built. The site is deployed via GitHub Actions.

### GitHub Actions Workflow

The workflow is defined at `.github/workflows/build.yml`; you can edit this file to change when the site is built or some aspects around its deployment.

It will run each day at midnight automatically, or when you push to the `main` branch. It sets up `dotnet` and `dotnet-script` so that it can run the CSX file. That setup is cached, so it will take longer to setup the first time but subsequent setups will only take a few seconds to fetch the cached files.

It then runs `build.csx`, which outputs the site to `/output`. The contents of that file are then committed onto the `gh-pages` branch, from which GitHub Pages will deploy.

## Contributing

Thank you for your interest, I would _love_ for you to contribute! If you have an idea for something to add please [open a discussion on it](https://github.com/IanWold/StaticBlogroll/discussions/new?category=ideas) and we can figure out next steps. I can't promise to include all features, my goal is to maintain this as a lightweight, super-easy way for folks to start a repo and get going.

If you've identified a bug please [open an issue](https://github.com/IanWold/StaticBlogroll/issues/new?assignees=IanWold&labels=&projects=&template=bug_report.md&title=) and I'll be sure to prioritize it. However, if you already have a simple fix it's definitely alright to open a PR off the bat!
