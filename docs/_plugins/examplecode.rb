module Jekyll
	class ExampleCodeBlock < Liquid::Block
        include Liquid::StandardFilters
		def render(context)
            code = super.join
            <<-HTML
<div class="highlight">
    <pre class="brush: csharp">#{h(code).strip}</pre>
</div>
            HTML
		end
	end
end
Liquid::Template.register_tag('examplecode', Jekyll::ExampleCodeBlock)
