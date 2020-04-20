# --------------------------------------------------------------------------- #
#
# Copyright (c) 2010 CubeSoft, Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#  http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
# --------------------------------------------------------------------------- #
require 'rake'
require 'rake/clean'

# --------------------------------------------------------------------------- #
# configuration
# --------------------------------------------------------------------------- #
PROJECT     = "Cube.Pdf.Apps"
BRANCHES    = ["master", "net35"]
FRAMEWORKS  = ["net45", "net35"]
CONFIGS     = ["Release", "Debug"]
PLATFORMS   = ["Any CPU", "x86", "x64"]
PACKAGES    = ["Libraries/Core/Cube.Pdf.Core.nuspec",
               "Libraries/Ghostscript/Cube.Pdf.Ghostscript.nuspec",
               "Libraries/Itext/Cube.Pdf.Itext.nuspec",
               "Libraries/Pdfium/Cube.Pdf.Pdfium.nuspec",
               "Applications/Converter/Core/Cube.Pdf.Converter.nuspec"]

# --------------------------------------------------------------------------- #
# unmanaged libraries
# --------------------------------------------------------------------------- #
COPIES = {
    "Cube.Native.Ghostscript/9.52.0" => [
        "Libraries/Tests",
        "Applications/Converter/Tests",
        "Applications/Converter/Main"
    ],
    "Cube.Native.Pdfium.Lite/1.0.4044" => [
        "Libraries/Tests",
        "Applications/Editor/Tests",
        "Applications/Editor/Main"
    ]
}

# --------------------------------------------------------------------------- #
# clean
# --------------------------------------------------------------------------- #
CLEAN.include(["*.nupkg", "**/bin", "**/obj"])
CLOBBER.include("../packages/cube.*")

# --------------------------------------------------------------------------- #
# default
# --------------------------------------------------------------------------- #
desc "Clean, build, test, and create NuGet packages."
task :default => [:clean] do
    Rake::Task[:build_all].invoke(false)
    checkout("net35") { Rake::Task[:pack].execute }
end

# --------------------------------------------------------------------------- #
# restore
# --------------------------------------------------------------------------- #
desc "Resote NuGet packages in the current branch."
task :restore do
    cmd("nuget restore #{PROJECT}.sln")
end

# --------------------------------------------------------------------------- #
# build
# --------------------------------------------------------------------------- #
desc "Build projects in the current branch."
task :build, [:platform] do |_, e|
    e.with_defaults(:platform => PLATFORMS[0])

    Rake::Task[:restore].execute
    branch = %x(git rev-parse --abbrev-ref HEAD).chomp
    build  = branch.start_with?("netstandard") || branch.start_with?("netcore") ?
             "dotnet build -c Release" :
             "msbuild -v:m -p:Configuration=Release"
    cmd(%(#{build} -p:Platform="#{e.platform}" #{PROJECT}.sln))
end

# --------------------------------------------------------------------------- #
# build_all
# --------------------------------------------------------------------------- #
desc "Build projects in pre-defined branches and platforms."
task :build_all, [:test] do |_, e|
    e.with_defaults(:test => false)
    
    BRANCHES.product(PLATFORMS).each do |bp|
        checkout(bp[0]) do
            Rake::Task[:build].reenable
            Rake::Task[:build].invoke(bp[1])
            Rake::Task[:test].execute if (e.test)
        end
    end
end

# --------------------------------------------------------------------------- #
# test
# --------------------------------------------------------------------------- #
desc "Test projects in the current branch."
task :test do
    cmd("dotnet test -c Release --no-restore --no-build #{PROJECT}.sln")
end

# --------------------------------------------------------------------------- #
# pack
# --------------------------------------------------------------------------- #
desc "Create NuGet packages."
task :pack do
    PACKAGES.each do |e|
        spec = File.exists?("#{e}.nuspec")
        pack = spec ?
               %(nuget pack -Properties "Configuration=Release;Platform=AnyCPU") :
               "dotnet pack -c Release --no-restore --no-build -o ."
        ext  = spec ? "nuspec" : "csproj"
        cmd("#{pack} #{e}.#{ext}")
    end
end

# --------------------------------------------------------------------------- #
# Copy
# --------------------------------------------------------------------------- #
desc "Copy umnamaged packages the bin directories."
task :copy, [:platform, :framework] => :restore do |_, e|
    v0 = (e.platform  != nil) ? [e.platform ] : PLATFORMS
    v1 = (e.framework != nil) ? [e.framework] : FRAMEWORKS

    v0.product(CONFIGS, v1) { |set|
        pf = (set[0] == 'Any CPU') ? 'x86' : set[0]
        COPIES.each { |key, value|
            src = FileList.new("../packages/#{key}/runtimes/win-#{pf}/native/*.dll")
            value.each { |root|
                dest = "#{root}/bin/#{set[0]}/#{set[1]}/#{set[2]}"
                RakeFileUtils::mkdir_p(dest)
                RakeFileUtils::cp_r(src, dest)
            }
        }
    }
end

# --------------------------------------------------------------------------- #
# checkout
# --------------------------------------------------------------------------- #
def checkout(branch, &callback)
    cmd("git checkout #{branch}")
    callback.call()
ensure
    cmd("git checkout master")
end

# --------------------------------------------------------------------------- #
# cmd
# --------------------------------------------------------------------------- #
def cmd(args)
    sh("cmd.exe /c #{args}")
end
