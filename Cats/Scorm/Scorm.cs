using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ADL.SCORM;
using ADL.Xml;
using ADL.Diagnostics;

namespace Scorm
{
    public class Scorm
    {
        private FileInfo pManifestFileInfo;
        private UserMessageCollection pMessages;
        private Manifest pManifest;

        public List<TreeActivity> TreeActivity { get; set; }

        public void OpenImsManifest(FileInfo file)
        {
            try
            {
                pManifestFileInfo = file;

                LoadImsManifest(true, true);

                TreeActivity = new List<TreeActivity>();

                foreach (var organization in pManifest.Organizations)
                {
                    if (organization.Children is {} && organization.Children.Any())
                    {
                        TreeActivityLoad(organization.Children);
                    }

                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void TreeActivityLoad(ItemCollection children)
        {
            foreach (Item child in children)
            {
                if (child.Children is {} && child.Children.Any())
                {
                    TreeActivityLoad(child.Children);
                }
                else
                {
                    var t = child.GetReferencedResource();

                    TreeActivity.Add(new TreeActivity
                    {
                        Name = child.Title,
                        Url = t.Href + child.Parameters,
                    });
                }
            }

        }

        private void LoadImsManifest(bool createDom, bool createData)
        {
            if (createDom)
            {
                CreateCourseDom();
            }

            #region Old Comment

            //if (createData)
            //{
            //    this.CreateCourseDataManager();
            //}

            //// Fire Event indicating that the course was loaded. Course loading and succesful sequencing are two different things.
            //this.OnCourseLoadingComplete(args);

            //// Start the sequencing session.
            //this.StartSequencingSession();

            #endregion
        }

        private void CreateCourseDom()
        {
            var d = new DomDocument<Manifest>();

            Namespaces.LoadNamespaceMappings(d);

            pMessages = new UserMessageCollection();

            if (d.Load(pMessages, pManifestFileInfo))
            {
                pManifest = d.DocumentElement;
            }
            else
            {
                throw new ApplicationException("The imsmanifet that was selected could not be loaded.");
            }
        }
    }
}
