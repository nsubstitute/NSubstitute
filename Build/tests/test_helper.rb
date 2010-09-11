require 'rubygems'
require 'spec'
require 'shoulda'
require 'rsubstitute'

def testing(library)
    require File.expand_path("../../lib/#{library}", __FILE__)
end

def equal_ignoring_whitespace(expected)
    MatchIgnoringWhitespace.new(expected)
end

private

class MatchIgnoringWhitespace
    def initialize(expected); @expected = expected; end

    def matches? subject
        raise "Can only compare collections of Strings" \
            unless subject.all? &is_string  and @expected.all? &is_string

        subject.map{ |x| x.strip }.should == @expected
    end

    private
    def is_string
        lambda { |x| x.class == String }
    end

end
