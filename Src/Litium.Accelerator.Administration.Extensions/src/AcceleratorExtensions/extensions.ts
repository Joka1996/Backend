import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { UiModule, PendingChangesGuard } from 'litium-ui';
import { FilteringComponent } from './components/filtering/filtering.component';
import { GroupingComponent } from './components/grouping/grouping.component';
import { SearchIndexingComponent } from './components/search-indexing/search-indexing.component';

const appRoutes = [
    {
        path: '',
        children: [
            { path: 'filtering', component: FilteringComponent, canDeactivate: [PendingChangesGuard] },
            { path: 'grouping', component: GroupingComponent, canDeactivate: [PendingChangesGuard] },
            { path: 'searchindexing', component: SearchIndexingComponent, canDeactivate: [PendingChangesGuard] },
        ]
    }
];

@NgModule({
    declarations: [
        FilteringComponent,
        GroupingComponent,
        SearchIndexingComponent,
    ],
    imports: [
        HttpClientModule,
        UiModule,
        TranslateModule,
        RouterModule.forChild(appRoutes),
    ]
})
export class AcceleratorExtensions {
}
