require 'rake/clean'
require File.expand_path('examples_to_code.rb', File.dirname(__FILE__))
require 'date'

DOT_NET_PATH = "#{ENV["SystemRoot"]}\\Microsoft.NET\\Framework\\v3.5"
NUNIT_EXE = "../ThirdParty/NUnit/bin/net-2.0/nunit-console.exe"
NUPACK_EXE = "../ThirdParty/NuPack/NuPack.exe"
SOURCE_PATH = "../Source"
OUTPUT_PATH = "../Output"

ENV["config"] = "Debug" if ENV["config"].nil?
CONFIG = ENV["config"]

CLEAN.include(OUTPUT_PATH)