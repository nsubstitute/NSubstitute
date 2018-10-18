# Use Kramdown, but add anchors with underscores for compatibility with older versions
class Jekyll::Converters::Markdown::KramdownPlus
  def initialize(config)
    require 'kramdown'
    @config = config
  rescue LoadError
    raise FatalException.new("Missing dependency: kramdown")
  end

  def convert(content)
    Kramdown::Document.new(content)
        .to_html
        .gsub(/<h\d id="(?<autoid>(\w|-)+)">.+<\/h\d>/) { |full_match|
            "<a id=\"#{$1.tr("-", "_")}\" />\n#{full_match}"
        }
  end
end
