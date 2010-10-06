A full build of NSubstitute, including document/website generation and packaging, requires the following:

* [Git](http://git-scm.com/). [Mysys git for Windows](http://code.google.com/p/msysgit/) works well. Git will need to be on your path.
* [Ruby 1.9+](http://ruby-lang.org). Using [RubyInstaller for Windows](http://rubyinstaller.org/) is probably easiest.
* [RubyInstaller DevKit](http://rubyinstaller.org/add-ons/devkit/) (see [wiki](http://github.com/oneclick/rubyinstaller/wiki/Development-Kit) for details)
* These Ruby gems: rubygems, rspec, shoulda, rsubstitute, jekyll (plus their dependencies)
* [Python](http://www.python.org/) (2.4 - 2.7, as required for Pygments. Not sure if it works with 3+)
* [Python setuptools](http://pypi.python.org/pypi/setuptools) to use easy\_install for Pygments
* [Pygments](http://pygments.org/) (for syntax highlighting in documentation)

*Note:* On my x64 Windows 7 I had trouble getting some of the Python stuff to install. To fix this I downloaded the _*.tar.gz*_ distribution and manually ran the setup scripts. For example, for _setuptools_ I downloaded [setuptools-0.6c11.tar.gz](http://pypi.python.org/pypi/setuptools#downloads), extracted it, and ran `python setup.py`.

