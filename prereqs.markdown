A full build of NSubstitute, including document/website generation and packaging, requires the following (in addition to .NET 3.5 development tools):

* [Git](http://git-scm.com/). [Mysys git for Windows](http://code.google.com/p/msysgit/) works well. Git will need to be on your path.
* [Ruby 1.9+](http://ruby-lang.org). Using [RubyInstaller for Windows](http://rubyinstaller.org/) is probably easiest.
* [RubyInstaller DevKit](http://rubyinstaller.org/add-ons/devkit/) (see [wiki](http://github.com/oneclick/rubyinstaller/wiki/Development-Kit) for details)
* These Ruby gems: rubygems, rake, rspec, shoulda, rsubstitute, jekyll (plus their dependencies)
* [Python](http://www.python.org/) (2.4 - 2.7, as required for Pygments. Not sure if it works with 3+)
* [Python setuptools](http://pypi.python.org/pypi/setuptools) to use easy\_install for Pygments
* [Pygments](http://pygments.org/) (for syntax highlighting in documentation)

*Note:* If you are using x64 Windows you may have trouble getting some of the Python dependencies to install. An alternative approach to using the setup programs is to download the _*.tar.gz*_ distributions and manually run the setup scripts. For example, for _setuptools_ download [setuptools-0.6c11.tar.gz](http://pypi.python.org/pypi/setuptools#downloads), extract it, and run `python setup.py`.

